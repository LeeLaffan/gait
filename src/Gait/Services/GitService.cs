using Gait.Utils;

namespace Gait.Services;

public class GitService(CommandRunner commandRunner, ConsoleOutput console)
{
    private const int MaxDirectoryTraversalDepth = 10;

    private string? ProjectRoot => field ??= GetProjectRoot();
    private readonly bool _staged = true;

    public Result<bool, string> Commit(string message)
    {
        if (string.IsNullOrWhiteSpace(ProjectRoot))
            return Result<bool, string>.Fail("Cannot retrieve git diff: Not in a valid project directory");

        var result = commandRunner.Run("git", $"commit -m '{message}'", ProjectRoot);
        if (!result.IsSuccess(out var _, out var error))
            return error;

        return true;
    }

    public Result<string, string> GetDiff()
    {
        if (string.IsNullOrWhiteSpace(ProjectRoot))
            return Result<string, string>.Fail("Cannot retrieve git diff: Not in a valid project directory");

        console.WriteProgress("Running `git diff`");

        var command = "diff" + (_staged ? " --staged" : string.Empty);
        var diffCommand = commandRunner.Run("git", command, ProjectRoot);

        if (diffCommand.IsError)
            return Result<string, string>.Fail($"Git command failed: \n{diffCommand.Error ?? "Unknown error"}");

        return diffCommand;
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
