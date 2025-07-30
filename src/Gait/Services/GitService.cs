using Gait.Utils;

namespace Gait.Services;

public class GitService(CommandRunner commandRunner, ConsoleOutput console)
{
    private const int MaxDirectoryTraversalDepth = 10;
    private readonly string? _projectRoot = GetProjectRoot();
    private readonly bool _staged = true;

    public Result<bool, string> Commit(string message)
    {
        if (string.IsNullOrWhiteSpace(_projectRoot))
            return Result<bool, string>.Fail("Cannot retrieve git diff: Not in a valid project directory");

        commandRunner.Run("git", "commit -m \"" + message + "\"", _projectRoot);

        return Result<bool, string>.Ok(true);
    }

    public Result<string, string> GetDiff()
    {
        if (string.IsNullOrWhiteSpace(_projectRoot))
            return Result<string, string>.Fail("Cannot retrieve git diff: Not in a valid project directory");

        console.WriteProgress("Running `git diff`");

        var command = "diff" + (_staged ? " --staged" : string.Empty);
        var diffCommand = commandRunner.Run("git", command, _projectRoot);

        if (diffCommand.ExitCode != 0)
            return Result<string, string>.Fail($"Git command failed: {diffCommand.Error ?? "Unknown error"}");

        if (string.IsNullOrWhiteSpace(diffCommand.Output))
            return Result<string, string>.Fail("No git diff output found");

        return Result<string, string>.Ok(diffCommand.Output ?? string.Empty);
    }

    private static string? GetProjectRoot()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(currentDirectory);
        var depth = 0;

        while (directory != null && depth < MaxDirectoryTraversalDepth)
        {
            depth++;

            if (IsGitRepository(directory.FullName))
                return directory.FullName; // Found

            directory = directory.Parent;
        }

        return null;
    }

    private static bool IsGitRepository(string directoryPath) => Directory.Exists(Path.Combine(directoryPath, ".git"));
}
