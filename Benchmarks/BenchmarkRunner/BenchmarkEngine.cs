using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkRunner.Models;

namespace BenchmarkRunner;

internal sealed class BenchmarkEngine(BenchmarkOptions options)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly string _fixtureRoot = Path.Combine(Path.GetTempPath(), $"alchemist-benchmarks-{Environment.ProcessId}");

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_fixtureRoot);
        
        var report = new BenchmarkReport
        {
            StartedAt = DateTimeOffset.UtcNow,
            CompletedAt = DateTimeOffset.UtcNow,
            Profile = options.Profile,
            AlchemistVersion = await ReadVersionAsync(options.AlchemistCommand, cancellationToken),
            DotnetVersion = await ReadVersionAsync("dotnet", cancellationToken),
            Results = []
        };

        try
        {
            var workload = CreateWorkload(options, report.StartedAt);
            
            Console.WriteLine(workload.Scenario);
            
            report.Results.Add(await RunWorkloadAsync(workload, cancellationToken));

            report.CompletedAt = DateTimeOffset.UtcNow;
            
            await WriteReportAsync(report, cancellationToken);
        }
        
        finally
        {
            if (options.KeepFixtures) Console.WriteLine($"Fixtures retained at {_fixtureRoot}.");
            
            else if (Directory.Exists(_fixtureRoot)) Directory.Delete(_fixtureRoot, recursive: true);
        }
    }

    private async Task<BenchmarkResult> RunWorkloadAsync(Workload workload, CancellationToken cancellationToken)
    {
        var root = Path.Combine(_fixtureRoot, workload.Scenario);
        
        Directory.CreateDirectory(root);
        
        var solution = FixtureGenerator.Create(root, 
            workload.ProjectCount, 
            workload.MethodCount);
        
        var process = await RunAlchemistAsync(solution, 
            workload.Framework, workload.Mode, 
            workload.Labels, cancellationToken);

        return CreateResult(workload, process);
    }

    private Task<(int ExitCode, TimeSpan Elapsed, string StandardOutput, string StandardError)> RunAlchemistAsync(string solution,
        UnitTestFramework framework,
        RegenerationMode mode,
        bool labels,
        CancellationToken cancellationToken) =>
            ProcessRunner.RunAsync(options.AlchemistCommand,
                [
                    "--solution", solution,
                    "--framework", FormatFramework(framework),
                    "--regeneration.mode", FormatMode(mode),
                    "--regeneration.labels", labels.ToString().ToLowerInvariant()
                ],
                Path.GetDirectoryName(solution)!,
                cancellationToken);

    private static BenchmarkResult CreateResult(Workload workload,
        (
            int ExitCode, 
            TimeSpan Elapsed, 
            string StandardOutput, 
            string StandardError
        ) 
        process)
            => new()
            {
                Scenario = workload.Scenario,
                Framework = workload.Framework,
                Mode = workload.Mode,
                Labels = workload.Labels,
                ProjectCount = workload.ProjectCount,
                MethodCount = workload.MethodCount,
                ElapsedMilliseconds = process.Elapsed.TotalMilliseconds,
                MethodsPerSecond = process.Elapsed.TotalSeconds <= 0 
                    ? 0 
                    : workload.MethodCount / process.Elapsed.TotalSeconds,
                ExitCode = process.ExitCode
            };

    private async Task WriteReportAsync(BenchmarkReport report, CancellationToken cancellationToken)
    {
        var date = FormatDate(report.StartedAt);
        var outputDirectory = Path.GetFullPath(Path.Combine("Benchmarks", 
            "Results", 
            date));
        
        var stem = $"benchmark-{FormatProfile(report.Profile)}-{FormatTimestamp(report.StartedAt)}";
        var jsonPath = Path.Combine(outputDirectory, $"{stem}.json");
        var csvPath = Path.Combine(outputDirectory, $"{stem}.csv");

        Directory.CreateDirectory(outputDirectory);
        
        await File.WriteAllTextAsync(jsonPath, 
            JsonSerializer.Serialize(report, JsonOptions), 
            cancellationToken);
        
        await File.WriteAllTextAsync(csvPath, CreateCsv(report), cancellationToken);

        Console.WriteLine();
        Console.WriteLine($"Results: {outputDirectory}");
        Console.WriteLine($"JSON: {jsonPath}");
        Console.WriteLine($"CSV:  {csvPath}");
        Console.WriteLine($"Completed {report.Results.Count} workloads.");
    }

    private static string CreateCsv(BenchmarkReport report)
    {
        var csv = new StringBuilder();
        
        csv.AppendLine("scenario,framework,mode,labels,projects,methods,elapsedMilliseconds,methodsPerSecond,exitCode");

        foreach (var result in report.Results)
        {
            csv.AppendLine(string.Join(",",
                Escape(result.Scenario),
                Escape(FormatFramework(result.Framework)),
                Escape(FormatMode(result.Mode)),
                result.Labels,
                result.ProjectCount,
                result.MethodCount,
                Format(result.ElapsedMilliseconds),
                Format(result.MethodsPerSecond),
                result.ExitCode));
        }

        return csv.ToString();
    }

    private static Workload CreateWorkload(BenchmarkOptions options, DateTimeOffset startedAt)
    {
        var (projects, methods) = ProfileSize(options.Profile);
        var scenario = $"{FormatProfile(options.Profile)}-{FormatTimestamp(startedAt)}";

        return new Workload(scenario,
            options.Framework,
            options.Mode,
            true,
            projects,
            methods);
    }

    private static (int Projects, int Methods) ProfileSize(BenchmarkProfile profile) =>
        profile switch
        {
            BenchmarkProfile.Small => (1, 100),
            BenchmarkProfile.Medium => (10, 1_000),
            BenchmarkProfile.Large => (100, 10_000),
            _ => throw new ArgumentOutOfRangeException(nameof(profile), profile, "Unknown benchmark profile.")
        };

    private static async Task<string> ReadVersionAsync(string command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await ProcessRunner.RunAsync(command, 
                ["--version"], 
                Directory.GetCurrentDirectory(), 
                cancellationToken);
            
            var version = result.StandardOutput.Trim();

            if (version.Length == 0) version = result.StandardError.Trim();

            return result.ExitCode == 0 
                ? version 
                : $"Unavailable: {Trim(version)}";
        }
        
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return $"Unavailable: {exception.Message}";
        }
    }

    private static string Escape(string value) =>
        value.IndexOfAny([',', '"', '\n', '\r']) >= 0
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;

    private static string Format(double value) =>
        value.ToString("0.###", CultureInfo.InvariantCulture);

    private static string FormatProfile(BenchmarkProfile profile) =>
        profile.ToString().ToLowerInvariant();

    private static string FormatTimestamp(DateTimeOffset value) =>
        value.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);

    private static string FormatDate(DateTimeOffset value) =>
        value.ToString("ddMMyyyy", CultureInfo.InvariantCulture);

    private static string FormatFramework(UnitTestFramework framework) =>
        framework switch
        {
            UnitTestFramework.XUnit => "xUnit",
            UnitTestFramework.NUnit => "NUnit",
            UnitTestFramework.MSTest => "MSTest",
            _ => throw new ArgumentOutOfRangeException(nameof(framework), framework, "Unknown test framework.")
        };

    private static string FormatMode(RegenerationMode mode) =>
        mode switch
        {
            RegenerationMode.Skip => "Skip",
            RegenerationMode.Update => "Update",
            RegenerationMode.Replace => "Replace",
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown regeneration mode.")
        };

    private static string Trim(string value) =>
        value.Length <= 4_000 ? value : value[..4_000];

    private sealed record Workload(string Scenario,
        UnitTestFramework Framework,
        RegenerationMode Mode,
        bool Labels,
        int ProjectCount,
        int MethodCount);
}
