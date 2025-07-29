namespace Gait.Services;

public interface IConfigurationService
{
    string GetOpenAIModel();
    string GetOpenAIKey();
}
