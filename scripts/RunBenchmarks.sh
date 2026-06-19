#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
project="$repo_root/Benchmarks/BenchmarkRunner/BenchmarkRunner.csproj"

exec dotnet run \
    --project "$project" \
    --configuration Release \
    -- "$@"
