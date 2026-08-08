#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
examples_dir="$repo_root/Examples"
alchemist_command=""
alchemist_tool_dir=""

frameworks=("xUnit" "NUnit" "MSTest")
modes=("Skip" "Update" "Replace")
labels=("true" "false")

project_guid="{8E6A08D8-0838-4E5B-A6E3-0229C3022C5D}"

print_usage() {
    cat <<'USAGE'
Usage:
  scripts/RegenerateExamples.sh [options]

Options:
  --alchemist <path>         Alchemist executable. Intended for a freshly
                             downloaded tool path from CI. If omitted, this
                             script installs the latest downloadable
                             KuBuCo.Alchemist package into a temporary tool path.
  -h, --help                 Show this help text.
USAGE
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        --alchemist)
            if [[ $# -lt 2 || "$2" == --* ]]; then
                printf 'Missing value for --alchemist.\n' >&2
                exit 1
            fi

            alchemist_command="$2"
            shift 2
            ;;
        -h|--help)
            print_usage
            exit 0
            ;;
        *)
            printf 'Unexpected argument: %s\n' "$1" >&2
            print_usage >&2
            exit 1
            ;;
    esac
done

staging_dir="${examples_dir}.tmp.$$"
backup_dir="${examples_dir}.backup.$$"

framework_directory() {
    case "$1" in
        xUnit)
            printf 'xUnit'
            ;;
        NUnit)
            printf 'nUnit'
            ;;
        MSTest)
            printf 'msTest'
            ;;
        *)
            printf 'Unsupported framework: %s\n' "$1" >&2
            exit 1
            ;;
    esac
}

scenario_directory() {
    local mode="$1"
    local labels_enabled="$2"

    if [[ "$labels_enabled" == "true" ]]; then
        printf '%sLabelsOn' "$mode"
    else
        printf '%sLabelsOff' "$mode"
    fi
}

resolve_alchemist() {
    if [[ -n "$alchemist_command" ]]; then
        return
    fi

    alchemist_tool_dir="${TMPDIR:-/tmp}/alchemist-regenerate-tools.$$"
    rm -rf "$alchemist_tool_dir"
    mkdir -p "$alchemist_tool_dir"

    dotnet tool install \
        --tool-path "$alchemist_tool_dir" \
        KuBuCo.Alchemist >&2

    alchemist_command="$alchemist_tool_dir/alchemist"
}

cleanup() {
    rm -rf "$staging_dir" "$alchemist_tool_dir"
}

trap cleanup EXIT

normalize_line_endings() {
    local target="$1"

    find "$target" -type f \( \
        -name '*.cs' -o \
        -name '*.csproj' -o \
        -name '*.sln' -o \
        -name '*.md' -o \
        -name '.alchemist-generated-files.json' \
    \) -exec perl -0pi -e 's/\r\n/\n/g' {} +
}

refresh_generated_file_manifest_hashes() {
    local target="$1"

    while IFS= read -r manifest; do
        MANIFEST_PATH="$manifest" perl \
            -MDigest::SHA=sha256_hex \
            -MFile::Basename=dirname \
            -MFile::Spec \
            -0777pi -e '
                BEGIN {
                    $manifest_path = $ENV{"MANIFEST_PATH"};
                    $solution_directory = dirname(dirname($manifest_path));
                }

                $entry_count = s{
                    ("Path":\s*"([^"]+)"\s*,\s*"ContentHash":\s*")
                    [0-9A-Fa-f]{64}
                    (")
                }{
                    @path_parts = split m{/}, $2;
                    $generated_path = File::Spec->catfile(
                        $solution_directory,
                        @path_parts);

                    open $generated_file, "<:raw", $generated_path
                        or die "Could not read $generated_path: $!";
                    local $/;
                    $content = <$generated_file>;
                    close $generated_file
                        or die "Could not close $generated_path: $!";

                    $1 . uc(sha256_hex($content)) . $3;
                }gex;

                die "No generated-file entries found in $manifest_path"
                    if $entry_count == 0;
            ' "$manifest"
    done < <(find "$target" \
        -path '*/UnitTests/.alchemist-generated-files.json' \
        -type f \
        | sort)
}

write_solution() {
    local destination="$1"

    cat > "$destination/Example.sln" <<SLN
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31903.59
MinimumVisualStudioVersion = 10.0.40219.1
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Example", "Example\\Example.csproj", "$project_guid"
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

    mkdir -p "$destination/Example"

    cat > "$destination/Example/Example.csproj" <<'CSPROJ'
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

    mkdir -p \
        "$destination/Example/Construction" \
        "$destination/Example/Operations"

    cat > "$destination/Example/Calculator.cs" <<'CS'
namespace Example;

public sealed class Calculator
{
    public int Add(int left, int right)
    {
        return left + right;
    }
}
CS

    cat > "$destination/Example/Construction/PrimitiveSubject.cs" <<'CS'
namespace Example.Construction;

public sealed class PrimitiveSubject
{
    private readonly int _count;

    public PrimitiveSubject(int count)
    {
        _count = count;
    }

    public int Measure()
    {
        return _count;
    }
}
CS

    cat > "$destination/Example/Operations/Multiplier.cs" <<'CS'
namespace Example.Operations;

public sealed class Multiplier
{
    public int Multiply(int left, int right)
    {
        return left * right;
    }
}
CS
}

write_expanded_source() {
    local destination="$1"

    mkdir -p \
        "$destination/Example/Construction" \
        "$destination/Example/Operations"

    cat > "$destination/Example/Calculator.cs" <<'CS'
namespace Example;

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

    cat > "$destination/Example/Construction/PrimitiveSubject.cs" <<'CS'
namespace Example.Construction;

public sealed class PrimitiveSubject
{
    private readonly int _count;

    public PrimitiveSubject(int count)
    {
        _count = count;
    }

    public int Measure()
    {
        return _count;
    }
}
CS

    cat > "$destination/Example/Operations/Multiplier.cs" <<'CS'
namespace Example.Operations;

public sealed class Multiplier
{
    public int Multiply(int left, int right)
    {
        return left * right;
    }

    public int Divide(int left, int right)
    {
        return left / right;
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
            old_assert='Assert.Fail("Scaffolded Unit Test");'
            new_assert='Assert.True(true); // manual-edit-preserved'
            ;;
        NUnit)
            old_assert='Assert.Fail("Scaffolded Unit Test");'
            new_assert='Assert.Pass(); // manual-edit-preserved'
            ;;
        MSTest)
            old_assert='Assert.Fail("Scaffolded Unit Test");'
            new_assert='Assert.Inconclusive("Manual edit preserved."); // manual-edit-preserved'
            ;;
        *)
            printf 'Unsupported framework: %s\n' "$framework" >&2
            exit 1
            ;;
    esac

    OLD_ASSERT="$old_assert" NEW_ASSERT="$new_assert" perl -0pi -e 's/\Q$ENV{OLD_ASSERT}\E/$ENV{NEW_ASSERT}/g' "$test_file"

    MANUAL_HELPER=$'\n\n        private void ManualHelper()\n        {\n        }\n' \
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
alchemist --solution ./Example.sln --framework $framework --regeneration.mode $mode --regeneration.labels $labels_enabled
\`\`\`

Inspect these generated files to see the final generated result:

- \`UnitTests/ExampleUnitTests/CalculatorUnitTests.cs\`
- \`UnitTests/ExampleUnitTests/Construction/PrimitiveSubjectUnitTests.cs\`
- \`UnitTests/ExampleUnitTests/Operations/MultiplierUnitTests.cs\`

Expected behavior:

- \`Skip\` keeps the existing generated \`Add_UnitTestPlaceholder\` method body and appends the missing \`Subtract_UnitTestPlaceholder\` method.
- \`Update\` regenerates the matching \`Add_UnitTestPlaceholder\` method, preserves \`ManualHelper\`, and appends \`Subtract_UnitTestPlaceholder\`.
- \`Replace\` overwrites the generated test file from the current source shape.
- Generated test files preserve the source-relative folder structure, so \`Example/Operations/Multiplier.cs\` generates \`UnitTests/ExampleUnitTests/Operations/MultiplierUnitTests.cs\`.
- Non-mockable constructor parameters use \`default\` arguments instead of invalid mock declarations, so \`PrimitiveSubject(int count)\` generates \`new PrimitiveSubject(default)\` without \`Mock<int>\`.
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

resolve_alchemist
rm -rf "$staging_dir" "$backup_dir"
mkdir -p "$staging_dir"

for framework in "${frameworks[@]}"; do
    framework_dir="$(framework_directory "$framework")"

    for mode in "${modes[@]}"; do
        for labels_enabled in "${labels[@]}"; do
            scenario_dir="$(scenario_directory "$mode" "$labels_enabled")"
            example_dir="$staging_dir/$framework_dir/$scenario_dir"

            mkdir -p "$example_dir"
            write_solution "$example_dir"
            write_project "$example_dir"
            write_initial_source "$example_dir"

            run_alchemist "$example_dir/Example.sln" "$framework" "Replace" "$labels_enabled" > /dev/null

            inject_manual_edits "$framework" "$example_dir/UnitTests/ExampleUnitTests/CalculatorUnitTests.cs"
            write_expanded_source "$example_dir"

            run_alchemist "$example_dir/Example.sln" "$framework" "$mode" "$labels_enabled" > /dev/null
            write_example_readme "$example_dir" "$framework" "$mode" "$labels_enabled"
        done
    done
done

find "$staging_dir" \( -name bin -o -name obj \) -type d -prune -exec rm -rf {} +
normalize_line_endings "$staging_dir"
refresh_generated_file_manifest_hashes "$staging_dir"

if [[ -e "$examples_dir" ]]; then
    mv "$examples_dir" "$backup_dir"
fi

if mv "$staging_dir" "$examples_dir"; then
    rm -rf "$backup_dir"
else
    if [[ -e "$backup_dir" ]]; then
        mv "$backup_dir" "$examples_dir"
    fi
    exit 1
fi

printf 'Generated %s example solutions in %s\n' "$(( ${#frameworks[@]} * ${#modes[@]} * ${#labels[@]} ))" "$examples_dir"
