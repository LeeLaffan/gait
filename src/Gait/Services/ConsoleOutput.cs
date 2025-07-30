using Microsoft.Extensions.Logging;

namespace Gait.Services;

public class ConsoleOutput(ILogger<ConsoleOutput> logger)
{
    public void WriteInfo(string message)
    {
        logger.LogInformation("Console Info: {Message}", message);
        Console.WriteLine(message);
    }

    public void WriteSuccess(string message)
    {
        logger.LogInformation("Console Success: {Message}", message);
        using var _ = new ConsoleColour(ConsoleColor.Green);
        Console.WriteLine(message);
    }

    public void WriteWarning(string message)
    {
        logger.LogWarning("Console Warning: {Message}", message);
        using var _ = new ConsoleColour(ConsoleColor.Yellow);
        Console.WriteLine(message);
    }

    public void WriteError(string message)
    {
        logger.LogError("Console Error: {Message}", message);
        using var _ = new ConsoleColour(ConsoleColor.Red);
        Console.WriteLine(message);
    }

    public void WriteLine(string message = "")
    {
        logger.LogDebug("Console WriteLine: {Message}", string.IsNullOrEmpty(message) ? "[empty line]" : message);
        Console.WriteLine(message);
    }

    public void Write(string message)
    {
        logger.LogDebug("Console Write: {Message}", message);
        Console.Write(message);
    }

    public void WriteProgress(string message)
    {
        logger.LogInformation("Console Progress: {Message}", message);
        using var _ = new ConsoleColour(ConsoleColor.Cyan);
        Console.WriteLine($"{message}");
    }

    /// Temporarily sets the console colour
    private class ConsoleColour : IDisposable
    {
        private readonly ConsoleColor _originalColour;

        public ConsoleColour(ConsoleColor foreground)
        {
            _originalColour = Console.ForegroundColor;
            Console.ForegroundColor = foreground;
        }

        public void Dispose() => Console.ForegroundColor = _originalColour;
    }
}
