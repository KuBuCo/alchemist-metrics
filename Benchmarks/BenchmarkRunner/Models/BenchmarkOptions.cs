namespace BenchmarkRunner.Models;

internal sealed record BenchmarkOptions(BenchmarkProfile Profile,
    string AlchemistCommand,
    UnitTestFramework Framework,
    RegenerationMode Mode,
    bool KeepFixtures);
