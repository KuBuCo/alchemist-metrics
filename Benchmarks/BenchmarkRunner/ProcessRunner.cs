using System.Diagnostics;

namespace BenchmarkRunner;

internal static class ProcessRunner
{
    public static async Task<(int ExitCode, TimeSpan Elapsed, string StandardOutput, string StandardError)> RunAsync(string fileName,
        IEnumerable<string> arguments,
        string workingDirectory,
        CancellationToken cancellationToken = default)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = new Process();
        
        process.StartInfo = startInfo;
        
        var stopwatch = Stopwatch.StartNew();
        
        process.Start();

        var standardOutputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var standardErrorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        
        stopwatch.Stop();

        return (process.ExitCode,
            stopwatch.Elapsed,
            await standardOutputTask,
            await standardErrorTask);
    }
}
