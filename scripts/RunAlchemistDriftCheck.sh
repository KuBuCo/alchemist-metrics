#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

alchemist_version="${ALCHEMIST_VERSION:-}"
alchemist_package_source="${ALCHEMIST_PACKAGE_SOURCE:-}"
alchemist_package_source_username="${ALCHEMIST_PACKAGE_SOURCE_USERNAME:-}"
alchemist_package_source_token="${ALCHEMIST_PACKAGE_SOURCE_TOKEN:-}"
alchemist_tool_path="${ALCHEMIST_TOOL_PATH:-${RUNNER_TEMP:-/tmp}/alchemist-tools}"
nuget_config_dir="${RUNNER_TEMP:-/tmp}/alchemist-nuget-config"

install_alchemist() {
    rm -rf "$alchemist_tool_path"
    mkdir -p "$alchemist_tool_path"

    local config_file=""
    if [[ -n "$alchemist_package_source" ]]; then
        rm -rf "$nuget_config_dir"
        mkdir -p "$nuget_config_dir"
        config_file="$nuget_config_dir/NuGet.config"

        cat > "$config_file" <<'XML'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
XML

        if [[ -n "$alchemist_package_source_token" ]]; then
            dotnet nuget add source "$alchemist_package_source" \
                --name AlchemistPackageSource \
                --configfile "$config_file" \
                --username "${alchemist_package_source_username:-github-actions}" \
                --password "$alchemist_package_source_token" \
                --store-password-in-clear-text >&2
        else
            dotnet nuget add source "$alchemist_package_source" \
                --name AlchemistPackageSource \
                --configfile "$config_file" >&2
        fi
    fi

    local install_args=(
        tool install
        --tool-path "$alchemist_tool_path"
        KuBuCo.Alchemist
    )

    if [[ -n "$alchemist_version" ]]; then
        install_args+=(--version "$alchemist_version")
    fi

    if [[ -n "$config_file" ]]; then
        install_args+=(--configfile "$config_file")
    fi

    dotnet "${install_args[@]}" >&2
    printf '%s\n' "$alchemist_tool_path/alchemist"
}

build_example_solutions() {
    while IFS= read -r solution; do
        dotnet restore "$solution"
        dotnet build "$solution" --no-restore
    done < <(find "$repo_root/Examples" -name Example.sln -type f | sort)
}

build_generated_test_projects() {
    while IFS= read -r project; do
        dotnet build "$project" --no-restore
    done < <(find "$repo_root/Examples" -path '*/UnitTests/UnitTests.csproj' -type f | sort)
}

check_generated_output_drift() {
    git diff --exit-code

    local untracked_files
    untracked_files="$(git ls-files --others --exclude-standard)"
    if [[ -n "$untracked_files" ]]; then
        printf 'Untracked files remain after regeneration:\n%s\n' "$untracked_files" >&2
        return 1
    fi
}

main() {
    cd "$repo_root"

    local command
    command="$(install_alchemist)"

    "$repo_root/scripts/RegenerateExamples.sh" --alchemist "$command"

    dotnet restore "$repo_root/AlchemistMetrics.sln"
    dotnet build "$repo_root/AlchemistMetrics.sln" --no-restore

    build_example_solutions
    build_generated_test_projects

    check_generated_output_drift
}

main "$@"
