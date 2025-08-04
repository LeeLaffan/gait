using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Gait.Configuration;
using Gait.Utils;

namespace Gait.Services;

public class AiService
{
    private readonly ILogger<AiService> _logger;
    private readonly OpenAIConfiguration _openAIOptions;
    private readonly ConsoleOutput _consoleOutput;
    private readonly IChatCompletionService _chatService;
    private readonly string _initialPrompt;

    public AiService(ILogger<AiService> logger, IOptions<OpenAIConfiguration> openAIOptions, ConsoleOutput consoleOutput)
    {
        _logger = logger;
        _openAIOptions = openAIOptions.Value;
        _consoleOutput = consoleOutput;

        _initialPrompt = LoadPromptFromFile(_openAIOptions.PromptPath) ?? "Summarize the below `git diff` output.";

        if (string.IsNullOrWhiteSpace(_openAIOptions.ApiKeyVar))
            throw new Exception($"OpenAI API key: {_openAIOptions.ApiKeyVar} not found in configuration variables");

        var apiKey = Environment.GetEnvironmentVariable(_openAIOptions.ApiKeyVar) ?? throw new Exception("OpenAI API key not found in environment variables");

        _chatService = new OpenAIChatCompletionService(_openAIOptions.Model, apiKey);
        _logger.LogInformation("AI Service initialized with model: {Model}", _openAIOptions.Model);
    }

    public async Task<Result<string, string>> GetDiffSummary(string diff)
    {
        var history = new ChatHistory();

        history.AddSystemMessage(_initialPrompt);
        history.AddUserMessage("Here is the `git diff` output:");
        history.AddUserMessage(diff);

        _consoleOutput.WriteProgress($"Summarising diff with model: {_openAIOptions.Model}");
        _consoleOutput.WriteLine();
        var response = string.Empty;
        await foreach (var res in _chatService.GetStreamingChatMessageContentsAsync(history))
        {
            if (res.Content == null)
                continue;

            _consoleOutput.Write(res.Content);
            response += res;
        }

        if (string.IsNullOrWhiteSpace(response))
            return Result<string, string>.Fail("Null response retrieved from AI");

        return Result<string, string>.Ok(response);
    }


    private string? LoadPromptFromFile(string promptPath)
    {
        if (string.IsNullOrWhiteSpace(promptPath))
        {
            _logger.LogWarning("No prompt path specified, using default prompt");
            return null;
        }

        try
        {
            if (!Path.IsPathRooted(promptPath))
                promptPath = Path.Combine(AppContext.BaseDirectory, promptPath);

            if (!File.Exists(promptPath))
            {
                _logger.LogWarning("Prompt file not found at {PromptPath}, using default prompt", promptPath);
                return null;
            }

            var prompt = File.ReadAllText(promptPath).Trim();
            _logger.LogInformation("Loaded prompt from file: {PromptPath}", promptPath);
            return prompt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading prompt from file {PromptPath}, using default prompt", promptPath);
            return null;
        }
    }
}
