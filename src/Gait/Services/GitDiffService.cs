using Microsoft.Extensions.Logging;

namespace Gait.Services;

public class GitDiffService(ILogger<GitDiffService> logger, AiService aiService, CommandRunner commandRunner)
{
    private const int MaxDirectoryTraversalDepth = 10;
    private readonly string _projectRoot = GetProjectRoot() ?? throw new InvalidOperationException("Project root not found");

    public async Task<string[]?> GetDiffSummaryAsync()
    {
        var diff = GetDiff();

        if (string.IsNullOrWhiteSpace(diff))
            return null;

        logger.LogInformation("Requesting AI summary for git diff");
        var summary = await aiService.GetDiffSummary(diff.Split('\n'));
        return summary;
    }

    public string GetDiff()
    {
        var result = commandRunner.Run("git", "diff", _projectRoot);
        if (result.ExitCode != 0 && !string.IsNullOrWhiteSpace(result.Error))
        {
            logger.LogError("Git diff failed: {Error}", result.Error);
            throw new InvalidOperationException($"Git diff failed: {result.Error}");
        }

        logger.LogInformation("Git diff retrieved successfully from {ProjectRoot}", _projectRoot);
        return result.Output;
    }

    private static string? GetProjectRoot()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var directory = new DirectoryInfo(currentDirectory);
        var depth = 0;

        while (directory != null && depth < MaxDirectoryTraversalDepth)
        {
            if (directory.GetFiles("*.sln").Any())
            {
                if (IsGitRepository(directory.FullName))
                {
                    return directory.FullName;
                }
                throw new InvalidOperationException("Not a git repository");
            }

            directory = directory.Parent;
            depth++;
        }

        return null;
    }

    private static bool IsGitRepository(string directoryPath) => Directory.Exists(Path.Combine(directoryPath, ".git"));
}
