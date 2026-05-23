#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
examples_dir="$repo_root/Examples"
alchemist_command="${ALCHEMIST_COMMAND:-alchemist}"

frameworks=("xUnit" "NUnit" "MSTest")
modes=("Skip" "Update" "Replace")
labels=("true" "false")

project_guid="{8E6A08D8-0838-4E5B-A6E3-0229C3022C5D}"

slug_framework() {
    printf '%s' "$1" | tr '[:upper:]' '[:lower:]'
}

slug_labels() {
    if [[ "$1" == "true" ]]; then
        printf 'labels-on'
    else
        printf 'labels-off'
    fi
}

write_solution() {
    local destination="$1"

    cat > "$destination/Demo.sln" <<SLN
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "DemoApp", "DemoApp\\DemoApp.csproj", "$project_guid"
EndProject
Global
    GlobalSection(SolutionConfigurationPlatforms) = preSolution
        Debug|Any CPU = Debug|Any CPU
        Release|Any CPU = Release|Any CPU
    EndGlobalSection
    GlobalSection(ProjectConfigurationPlatforms) = postSolution
        $project_guid.Debug|Any CPU.ActiveCfg = Debug|Any CPU
        $project_guid.Debug|Any CPU.Build.0 = Debug|Any CPU
        $project_guid.Release|Any CPU.ActiveCfg = Release|Any CPU
        $project_guid.Release|Any CPU.Build.0 = Release|Any CPU
    EndGlobalSection
EndGlobal
SLN
}

write_project() {
    local destination="$1"

    mkdir -p "$destination/DemoApp"

    cat > "$destination/DemoApp/DemoApp.csproj" <<'CSPROJ'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
CSPROJ
}

write_initial_source() {
    local destination="$1"

    cat > "$destination/DemoApp/Calculator.cs" <<'CS'
namespace DemoApp;

public sealed class Calculator
{
    public int Add(int left, int right)
    {
        return left + right;
    }
}
CS
}

write_expanded_source() {
    local destination="$1"

    cat > "$destination/DemoApp/Calculator.cs" <<'CS'
namespace DemoApp;

public sealed class Calculator
{
    public int Add(int left, int right)
    {
        return left + right;
    }

    public int Subtract(int left, int right)
    {
        return left - right;
    }
}
CS
}

inject_manual_edits() {
    local framework="$1"
    local test_file="$2"
    local old_assert
    local new_assert

    case "$framework" in
        xUnit)
            old_assert='Assert.True(false, "Scaffolded Unit Test");'
            new_assert='Assert.True(true); // manual-edit-preserved'
            ;;
        NUnit)
            old_assert='Assert.Fail("Scaffolded Unit Test");'
            new_assert='Assert.Pass(); // manual-edit-preserved'
            ;;
        MSTest)
            old_assert='Assert.Fail("Scaffolded Unit Test");'
            new_assert='Assert.IsTrue(true); // manual-edit-preserved'
            ;;
        *)
            printf 'Unsupported framework: %s\n' "$framework" >&2
            exit 1
            ;;
    esac

    OLD_ASSERT="$old_assert" NEW_ASSERT="$new_assert" perl -0pi -e 's/\Q$ENV{OLD_ASSERT}\E/$ENV{NEW_ASSERT}/g' "$test_file"

    MANUAL_HELPER=$'\n\n        public void ManualHelper()\n        {\n        }\n' \
        perl -0pi -e 's/(\r?\n    }\r?\n}\s*)\z/$ENV{MANUAL_HELPER}$1/s' "$test_file"
}

write_example_readme() {
    local destination="$1"
    local framework="$2"
    local mode="$3"
    local labels_enabled="$4"

    cat > "$destination/README.md" <<README
# $framework / $mode / labels=$labels_enabled

This example demonstrates:

- Framework: \`$framework\`
- Regeneration mode: \`$mode\`
- Regeneration labels: \`$labels_enabled\`

Command used:

\`\`\`bash
alchemist --solution ./Demo.sln --framework $framework --regeneration.mode $mode --regeneration.labels $labels_enabled
\`\`\`

Inspect \`UnitTests/DemoAppUnitTests/CalculatorUnitTests.cs\` to see the final generated result.

Expected behavior:

- \`Skip\` keeps the existing generated \`Add_UnitTestPlaceholder\` method body and appends the missing \`Subtract_UnitTestPlaceholder\` method.
- \`Update\` regenerates the matching \`Add_UnitTestPlaceholder\` method, preserves \`ManualHelper\`, and appends \`Subtract_UnitTestPlaceholder\`.
- \`Replace\` overwrites the generated test file from the current source shape.
- \`labels=true\` emits \`UnitTestID\` comments on generated methods.
- \`labels=false\` omits \`UnitTestID\` comments and relies on method names for update matching.
README
}

run_alchemist() {
    local solution_path="$1"
    local framework="$2"
    local mode="$3"
    local labels_enabled="$4"

    "$alchemist_command" \
        --solution "$solution_path" \
        --framework "$framework" \
        --regeneration.mode "$mode" \
        --regeneration.labels "$labels_enabled"
}

rm -rf "$examples_dir"
mkdir -p "$examples_dir"

for framework in "${frameworks[@]}"; do
    framework_slug="$(slug_framework "$framework")"

    for mode in "${modes[@]}"; do
        mode_slug="$(slug_framework "$mode")"

        for labels_enabled in "${labels[@]}"; do
            label_slug="$(slug_labels "$labels_enabled")"
            example_dir="$examples_dir/$framework_slug-$mode_slug-$label_slug"

            mkdir -p "$example_dir"
            write_solution "$example_dir"
            write_project "$example_dir"
            write_initial_source "$example_dir"

            run_alchemist "$example_dir/Demo.sln" "$framework" "Replace" "$labels_enabled" > "$example_dir/initial-run.txt"

            inject_manual_edits "$framework" "$example_dir/UnitTests/DemoAppUnitTests/CalculatorUnitTests.cs"
            write_expanded_source "$example_dir"

            run_alchemist "$example_dir/Demo.sln" "$framework" "$mode" "$labels_enabled" > "$example_dir/final-run.txt"
            write_example_readme "$example_dir" "$framework" "$mode" "$labels_enabled"
        done
    done
done

find "$examples_dir" \( -name bin -o -name obj \) -type d -prune -exec rm -rf {} +

printf 'Generated %s example solutions in %s\n' "$(( ${#frameworks[@]} * ${#modes[@]} * ${#labels[@]} ))" "$examples_dir"
