using Gait.Services;
using Microsoft.Extensions.Logging;

namespace Gait;

public class App(ILogger<App> logger, GitDiffService gitDiffService)
{
    public async Task RunAsync()
    {
        logger.LogInformation("Starting Gait application...");

        try
        {
            var diff = gitDiffService.GetDiff();
            if (string.IsNullOrWhiteSpace(diff))
            {
                logger.LogWarning("No git diff available - either no .sln file found, not in a git repository, or no changes detected");
                return;
            }

            logger.LogInformation("Git diff retrieved successfully");
            logger.LogDebug("Git diff output: {GitDiff}", diff);

            logger.LogInformation("Requesting AI summary for git diff");
            var summary = await gitDiffService.GetDiffSummaryAsync();
            logger.LogInformation("AI Git Diff Summary:");
            logger.LogInformation("");
            logger.LogInformation("");
            foreach (var line in summary)
                logger.LogInformation(line);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing git diff: {ErrorMessage}", ex.Message);
        }

        logger.LogInformation("Gait application completed.");
    }
}
