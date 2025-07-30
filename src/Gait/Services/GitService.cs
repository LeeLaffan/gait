using Gait.Utils;
using Microsoft.Extensions.Logging;

namespace Gait.Services;

public class GitService(ILogger<GitService> logger, CommandRunner commandRunner, ConsoleOutput console)
{
    private const int MaxDirectoryTraversalDepth = 10;

    private string? ProjectRoot => field ??= GetProjectRoot();
    private readonly bool _staged = true;

    public Result<bool, string> Commit(string message)
    {
        if (string.IsNullOrWhiteSpace(ProjectRoot))
            return Result<bool, string>.Fail("Cannot git commit: Not in a valid project directory");

        // Split message into lines and build arguments
        var lines = message.Split(["\r\n", "\n"], StringSplitOptions.None);
        var args = string.Join(" ", lines.Select(line => $"-m \"{line}\""));

        args = """ -m "hi" -m "bye" """;

        var result = commandRunner.Run("git", $"commit {args}", ProjectRoot);
        if (result.IsError(out _, out var error))
            return error;

        return true;
    }

    public Result<bool, string> AddAll()
    {
        if (string.IsNullOrWhiteSpace(ProjectRoot))
            return Result<bool, string>.Fail("Cannot git add: Not in a valid project directory");

        var result = commandRunner.Run("git", "add *", ProjectRoot);
        if (result.IsError(out var success, out var error))
            return error;

        return true;
    }

    public Result<bool, string> Push()
    {
        if (string.IsNullOrWhiteSpace(ProjectRoot))
            return Result<bool, string>.Fail("Cannot git push: Not in a valid project directory");

        var result = commandRunner.Run("git", "push", ProjectRoot);
        if (result.IsError(out var success, out var error))
            return error;

        return true;
    }

    public Result<string, string> GetDiff()
    {
        if (string.IsNullOrWhiteSpace(ProjectRoot))
            return Result<string, string>.Fail("Cannot git diff: Not in a valid project directory");

        console.WriteProgress("Running `git diff`");

        var command = "diff" + (_staged ? " --staged" : string.Empty);
        var diffResult = commandRunner.Run("git", command, ProjectRoot);

        if (diffResult.IsError(out var diff, out var error))
            return Result<string, string>.Fail($"Git command failed:\n{error}");

        if (string.IsNullOrWhiteSpace(diff))
            return Result<string, string>.Fail("No git diff found");

        logger.LogInformation("Git diff retrieved successfully");
        logger.LogDebug("Git diff output: {GitDiff}", diffResult);

        return diffResult;
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
