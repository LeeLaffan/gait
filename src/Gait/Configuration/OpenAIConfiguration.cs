namespace Gait.Configuration;

public class OpenAIConfiguration
{
    public const string SectionName = "OpenAI";
    
    public string Model { get; set; } = "gpt-4o-mini";
    public string ApiKey { get; set; } = string.Empty;
    public string PromptPath { get; set; } = string.Empty;
}
