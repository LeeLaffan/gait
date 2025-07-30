using Microsoft.Extensions.Logging;

namespace Gait.Services;

public class ConsoleOutput
{
    private readonly ILogger<ConsoleOutput> _logger;

    public ConsoleOutput(ILogger<ConsoleOutput> logger) => _logger = logger;
    public void WriteInfo(string message)
    {
        _logger.LogInformation("Console Info: {Message}", message);
        Console.WriteLine($"ℹ️  {message}");
    }

    public void WriteSuccess(string message)
    {
        _logger.LogInformation("Console Success: {Message}", message);
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✅ {message}");
        Console.ForegroundColor = originalColor;
    }

    public void WriteWarning(string message)
    {
        _logger.LogWarning("Console Warning: {Message}", message);
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"⚠️  {message}");
        Console.ForegroundColor = originalColor;
    }

    public void WriteError(string message)
    {
        _logger.LogError("Console Error: {Message}", message);
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"❌ {message}");
        Console.ForegroundColor = originalColor;
    }

    public void WriteLine(string message = "")
    {
        _logger.LogDebug("Console WriteLine: {Message}", string.IsNullOrEmpty(message) ? "[empty line]" : message);
        Console.WriteLine(message);
    }

    public void Write(string message)
    {
        Console.Write(message);
        _logger.LogDebug("Console Write: {Message}", message);
    }

    public void WriteProgress(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"🔄 {message}");
        Console.ForegroundColor = originalColor;
        _logger.LogInformation("Console Progress: {Message}", message);
    }
}
