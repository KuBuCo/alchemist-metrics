using System.Text;

namespace BenchmarkRunner;

internal static class FixtureGenerator
{
    public static string Create(string root, 
        int projectCount, 
        int methodCount)
    {
        Directory.CreateDirectory(root);
        
        WriteSolution(root, projectCount);
        WriteProjects(root, projectCount);
        WriteSources(root, 
            projectCount, 
            methodCount);
        
        return Path.Combine(root, "Benchmark.sln");
    }

    private static void WriteSources(string root, 
        int projectCount, 
        int methodCount)
    {
        var distribution = Distribute(methodCount, projectCount);

        for (var projectIndex = 0; projectIndex < projectCount; projectIndex++)
        {
            var projectName = ProjectName(projectIndex);
            var sourcePath = Path.Combine(root, 
                projectName, 
                $"Service{projectIndex:D3}.cs");
            
            var source = new StringBuilder();

            source.AppendLine($"namespace {projectName};");
            source.AppendLine();
            source.AppendLine("public readonly record struct SharedValue(int Value);");
            source.AppendLine();
            source.AppendLine($"public sealed class Service{projectIndex:D3}");
            source.AppendLine("{");

            for (var methodIndex = 0; methodIndex < distribution[projectIndex]; methodIndex++)
            {
                if (projectIndex > 0 && methodIndex == 0)
                {
                    source.AppendLine($"    public int Method{methodIndex:D5}({ProjectName(projectIndex - 1)}.SharedValue value)");
                    source.AppendLine("    {");
                    source.AppendLine($"        return value.Value + {projectIndex};");
                }
                
                else
                {
                    source.AppendLine($"    public int Method{methodIndex:D5}(int value)");
                    source.AppendLine("    {");
                    source.AppendLine($"        return value + {projectIndex + methodIndex};");
                }

                source.AppendLine("    }");
                source.AppendLine();
            }

            source.AppendLine("}");
            
            File.WriteAllText(sourcePath, source.ToString());
        }
    }

    private static void WriteProjects(string root, int projectCount)
    {
        for (var index = 0; index < projectCount; index++)
        {
            var projectName = ProjectName(index);
            var projectDirectory = Path.Combine(root, projectName);
            
            Directory.CreateDirectory(projectDirectory);

            var projectReference = index == 0
                ? string.Empty
                : $"""
                  <ItemGroup>
                    <ProjectReference Include="../{ProjectName(index - 1)}/{ProjectName(index - 1)}.csproj" />
                  </ItemGroup>
                """;

            var project = $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>net8.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                {projectReference}
                </Project>
                """;

            File.WriteAllText(Path.Combine(projectDirectory, $"{projectName}.csproj"), project);
        }
    }

    private static void WriteSolution(string root, int projectCount)
    {
        var solution = new StringBuilder();
        
        solution.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
        solution.AppendLine("# Visual Studio Version 17");
        solution.AppendLine("VisualStudioVersion = 17.0.31903.59");
        solution.AppendLine("MinimumVisualStudioVersion = 10.0.40219.1");

        for (var index = 0; index < projectCount; index++)
        {
            var projectName = ProjectName(index);
            var guid = ProjectGuid(index);
            
            solution.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{projectName}\", \"{projectName}\\{projectName}.csproj\", \"{guid}\"");
            solution.AppendLine("EndProject");
        }

        solution.AppendLine("Global");
        solution.AppendLine("    GlobalSection(SolutionConfigurationPlatforms) = preSolution");
        solution.AppendLine("        Debug|Any CPU = Debug|Any CPU");
        solution.AppendLine("        Release|Any CPU = Release|Any CPU");
        solution.AppendLine("    EndGlobalSection");
        solution.AppendLine("    GlobalSection(ProjectConfigurationPlatforms) = postSolution");

        for (var index = 0; index < projectCount; index++)
        {
            var guid = ProjectGuid(index);
            
            solution.AppendLine($"        {guid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            solution.AppendLine($"        {guid}.Debug|Any CPU.Build.0 = Debug|Any CPU");
            solution.AppendLine($"        {guid}.Release|Any CPU.ActiveCfg = Release|Any CPU");
            solution.AppendLine($"        {guid}.Release|Any CPU.Build.0 = Release|Any CPU");
        }

        solution.AppendLine("    EndGlobalSection");
        solution.AppendLine("EndGlobal");
        
        File.WriteAllText(Path.Combine(root, "Benchmark.sln"), solution.ToString());
    }

    private static int[] Distribute(int methodCount, int projectCount)
    {
        var result = new int[projectCount];
        
        for (var index = 0; index < methodCount; index++)
        {
            result[index % projectCount]++;
        }

        return result;
    }

    private static string ProjectName(int index) => 
        $"Project{index:D3}";

    private static string ProjectGuid(int index) =>
        $"{{8E6A08D8-0838-4E5B-A6E3-{index + 1:D12}}}";

}
