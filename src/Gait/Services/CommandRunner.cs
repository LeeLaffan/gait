using System.Diagnostics;
using Gait.Utils;

namespace Gait.Services;

public class CommandRunner
{
    public Result<string, string> Run(string fileName, string arguments, string workingDirectory)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        return process.ExitCode == 0
            ? Result<string, string>.Ok(output + error)
            : Result<string, string>.Fail(error);
    }
}

public record CommandResult(int ExitCode, string? Output, string? Error);
