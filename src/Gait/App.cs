using Gait.Services;
using Microsoft.Extensions.Logging;

namespace Gait;

public class App(ILogger<App> logger, GitDiffService gitDiffService, ConsoleOutput console)
{
    public async Task RunAsync()
    {
        console.WriteInfo("Starting Gait application...");
        logger.LogInformation("Starting Gait application...");

        var diff = gitDiffService.GetDiff();
        if (diff.IsError)
        {
            console.WriteInfo(diff.Error);
            Environment.Exit(1);
        }

        logger.LogInformation("Git diff retrieved successfully");
        logger.LogDebug("Git diff output: {GitDiff}", diff);

        console.WriteProgress("Analysing git diff with AI...");
        var summary = await gitDiffService.GetDiffSummaryAsync();
        console.WriteSuccess("AI analysis completed successfully");

        if (summary.IsError)
        {
            console.WriteWarning("No AI summary could be generated");
            Environment.Exit(1);
        }

        console.WriteLine();
        console.WriteSuccess("AI Git Diff Summary:");
        console.WriteLine();

        logger.LogInformation("AI Git Diff Summary:");
        foreach (var line in summary.Value)
        {
            console.WriteLine(line);
            logger.LogInformation(line);
        }

        console.WriteLine();
        console.WriteSuccess("Gait application completed successfully");
        logger.LogInformation("Gait application completed.");
    }
}
