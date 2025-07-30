using Gait.Configuration;
using Gait.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gait;

public class App(ILogger<App> logger, GitService gitService, ConsoleOutput console, AiService aiService, IOptions<OpenAIConfiguration> aiOptions)
{
    public async Task RunAsync()
    {
        var diff = gitService.GetDiff();
        if (diff.IsError)
        {
            console.WriteError(diff.Error);
            Environment.Exit(1);
        }

        logger.LogInformation("Git diff retrieved successfully");
        logger.LogDebug("Git diff output: {GitDiff}", diff);

        console.WriteProgress($"Summarising diff with model: {aiOptions.Value.Model}");
        var summary = await aiService.GetDiffSummary(diff.Value);
        console.WriteSuccess("AI analysis completed successfully");

        if (string.IsNullOrWhiteSpace(summary))
        {
            console.WriteWarning("No AI summary could be generated");
            Environment.Exit(1);
        }

        if (!gitService.Commit("hi").IsSuccess(out var _, out var error))
        {
            console.WriteError($"Error running commit command:\n{error}");
            Environment.Exit(1);
        }

        console.WriteLine();
        console.WriteSuccess("Gait application completed successfully");
    }
}
