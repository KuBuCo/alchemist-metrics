using System;
using System.Collections.Generic;

namespace BenchmarkRunner.Models;

internal sealed class BenchmarkReport
{
    public required DateTimeOffset StartedAt { get; init; }
    public required DateTimeOffset CompletedAt { get; set; }
    public required BenchmarkProfile Profile { get; init; }
    public required string AlchemistVersion { get; init; }
    public required string DotnetVersion { get; init; }
    public required List<BenchmarkResult> Results { get; init; }
}
