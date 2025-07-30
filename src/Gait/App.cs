using Gait.Configuration;
using Gait.Services;
using Gait.Utils;
using Microsoft.Extensions.Options;

namespace Gait;

public class App(GitService gitService, ConsoleOutput console, AiService aiService, IOptions<OpenAIConfiguration> aiOptions, IOptions<GitConfiguration> gitOptions)
{
    public async Task RunAsync() =>
        (await Process()).Match(
            console.WriteSuccess,
            error =>
            {
                console.WriteError(error);
                Environment.Exit(1);
            });

    private async Task<Result<string, string>> Process()
    {
        if (gitService.AddAll().IsError(out _, out var error))
            return Result<string, string>.Fail($"Error running Add: {error}");

        if (gitService.GetDiff().IsError(out var diff, out error))
            return Result<string, string>.Fail($"Error running Diff: {error}");

        if ((await aiService.GetDiffSummary(diff)).IsError(out var summary, out error))
            return Result<string, string>.Fail($"No AI summary could be generated: {error}");

        if (!gitService.Commit(AppendSignature(summary)).IsSuccess(out _, out error))
            return Result<string, string>.Fail($"Error running commit command:\n{error}");

        if (!gitService.Push().IsSuccess(out _, out error))
            return Result<string, string>.Fail($"Error running push command:\n{error}");

        return Result<string, string>.Ok("Gait successfully commited changes");
    }

    private string AppendSignature(string summary)
    {
        var signature = gitOptions.Value.CommitSignature ?? string.Empty;

        if (string.IsNullOrWhiteSpace(signature))
            return summary;

        console.WriteLine(signature);
        console.WriteLine();
        return summary + signature;
    }
}
