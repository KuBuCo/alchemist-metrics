using BenchmarkRunner;
using BenchmarkRunner.Models;

const string UnexpectedArgumentMessage = "Unexpected argument: {0}";
const string DuplicateOptionMessage = "Option {0} can be configured once.";
const string MissingValueMessage = "Missing value for {0}.";
const string InvalidProfileMessage = "Profile must be small, medium or large.";
const string InvalidFrameworkMessage = "Framework must be xunit, nunit or mstest.";
const string InvalidModeMessage = "Mode must be skip, update or replace.";
const string MissingAlchemistMessage = "Unable to find alchemist, install kubuco.alchemist or input --alchemist <path>.";

try
{
    if (args is ["--help"] or ["-h"])
    {
        PrintUsage();

        return 0;
    }

    var options = ParseOptions(args);
    var engine = new BenchmarkEngine(options);
    
    await engine.RunAsync();

    return 0;
}

catch (Exception exception) when (exception is ArgumentException or FileNotFoundException)
{
    Console.Error.WriteLine(exception.Message);
    Console.Error.WriteLine();
    
    PrintUsage();

    return 1;
}

static BenchmarkOptions ParseOptions(string[] args)
{
    var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    var keepFixtures = false;
    
    for (var index = 0; index < args.Length; index++)
    {
        var argument = args[index];
        
        if (!argument.StartsWith("--", StringComparison.Ordinal))
        {
            throw new ArgumentException(string.Format(UnexpectedArgumentMessage, argument));
        }

        if (argument is "--keep-fixtures")
        {
            if (keepFixtures)
            {
                throw new ArgumentException(string.Format(DuplicateOptionMessage, "--keep-fixtures"));
            }

            keepFixtures = true;
            
            continue;
        }

        if (argument is not ("--profile" or "--alchemist" or "--framework" or "--mode"))
        {
            throw new ArgumentException(string.Format(UnexpectedArgumentMessage, argument));
        }

        if (index + 1 >= args.Length)
        {
            throw new ArgumentException(string.Format(MissingValueMessage, argument));
        }

        if (values.ContainsKey(argument))
        {
            throw new ArgumentException(string.Format(DuplicateOptionMessage, argument));
        }

        var value = args[++index];

        if (value.StartsWith("--", StringComparison.Ordinal))
        {
            throw new ArgumentException(string.Format(MissingValueMessage, argument));
        }

        values[argument] = value;
    }

    var profile = ParseProfile(Get(values, "--profile") ?? "small");
    var framework = ParseFramework(Get(values, "--framework") ?? "xunit");
    var mode = ParseMode(Get(values, "--mode") ?? "replace");
    var alchemist = Get(values, "--alchemist") ?? ResolveAlchemist();

    return new BenchmarkOptions(profile,
        alchemist,
        framework,
        mode,
        keepFixtures);
}

static string? Get(Dictionary<string, string> values, string key) =>
    values.GetValueOrDefault(key);

static BenchmarkProfile ParseProfile(string value) =>
    value.ToLowerInvariant() switch
    {
        "small" => BenchmarkProfile.Small,
        "medium" => BenchmarkProfile.Medium,
        "large" => BenchmarkProfile.Large,
        _ => throw new ArgumentException(InvalidProfileMessage)
    };

static UnitTestFramework ParseFramework(string value) =>
    value.ToLowerInvariant() switch
    {
        "xunit" => UnitTestFramework.XUnit,
        "nunit" => UnitTestFramework.NUnit,
        "mstest" => UnitTestFramework.MSTest,
        _ => throw new ArgumentException(InvalidFrameworkMessage)
    };

static RegenerationMode ParseMode(string value) =>
    value.ToLowerInvariant() switch
    {
        "skip" => RegenerationMode.Skip,
        "update" => RegenerationMode.Update,
        "replace" => RegenerationMode.Replace,
        _ => throw new ArgumentException(InvalidModeMessage)
    };

static string ResolveAlchemist()
{
    var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
    
    foreach (var directory in path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
    {
        var candidate = Path.Combine(directory, AlchemistFileName());
        
        if (File.Exists(candidate)) return candidate;
    }

    var globalTool = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".dotnet",
        "tools",
        AlchemistFileName());
    
    return File.Exists(globalTool) 
        ? globalTool 
        : throw new FileNotFoundException(MissingAlchemistMessage);
}

static string AlchemistFileName() =>
    OperatingSystem.IsWindows() ? "alchemist.exe" : "alchemist";

static void PrintUsage()
{
    Console.WriteLine("""
        Usage:
          Scripts/RunBenchmarks.sh [options]

        Options:
          --profile <small|medium|large>   Benchmark size, defaults to small.
          --framework <xunit|nunit|mstest> Test framework, defaults to xunit.
          --mode <skip|update|replace>     Regeneration mode, defaults to replace.
          --alchemist <path-or-command>    Alchemist executable, defaults to path lookup.
          --keep-fixtures                  Keep benchmark projects.
          --help                           Show this help text.

        """);
}
