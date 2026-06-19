namespace BenchmarkRunner.Models;

internal sealed class BenchmarkResult
{
    public required string Scenario { get; init; }
    public required UnitTestFramework Framework { get; init; }
    public required RegenerationMode Mode { get; init; }
    public required bool Labels { get; init; }
    public required int ProjectCount { get; init; }
    public required int MethodCount { get; init; }
    public required double ElapsedMilliseconds { get; init; }
    public required double MethodsPerSecond { get; init; }
    public required int ExitCode { get; init; }
}
