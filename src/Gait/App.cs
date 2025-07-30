using Gait.Configuration;
using Gait.Services;
using Microsoft.Extensions.Options;

namespace Gait;

public class App(GitService gitService, ConsoleOutput console, AiService aiService, IOptions<OpenAIConfiguration> aiOptions)
{
    public async Task RunAsync()
    {
        var add = gitService.AddAll();
        if (add.IsError(out _, out var error))
        {
            console.WriteError(error);
            Environment.Exit(1);
        }

        var diffResult = gitService.GetDiff();
        if (diffResult.IsError(out var diff, out error))
        {
            console.WriteError(error);
            Environment.Exit(1);
        }

        console.WriteProgress($"Summarising diff with model: {aiOptions.Value.Model}");
        var summary = await aiService.GetDiffSummary(diff);
        if (string.IsNullOrWhiteSpace(summary))
        {
            console.WriteWarning("No AI summary could be generated");
            Environment.Exit(1);
        }

        if (!gitService.Commit(summary).IsSuccess(out _, out error))
        {
            console.WriteError($"Error running commit command:\n{error}");
            Environment.Exit(1);
        }

        if (!gitService.Push().IsSuccess(out _, out error))
        {
            console.WriteError($"Error running push command:\n{error}");
            Environment.Exit(1);
        }

        console.WriteSuccess("Gait successfully commit changes");
    }
}
