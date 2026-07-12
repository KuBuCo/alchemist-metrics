# Alchemist Metrics

Public integration fixture and companion examples for
[`KuBuCo.Alchemist`](https://kubuco.github.io/alchemist/readme/).

The public documentation is the source of truth for what the tool does, how to 
install it and how its command-line options work. This repository exists to make
that behavior inspectable and testable outside the main Alchemist repository: it
contains small source projects together with the `UnitTests` project generated
for each supported test framework, regeneration mode and label setting.

Generated output is intentionally checked in. Diffs in `Examples/**/UnitTests`,
the generated `Example.sln` entries, or generated project files are behavior
changes and should be reviewed rather than hidden with `.gitignore` rules.

## What This Adds

- A concrete example matrix for `xUnit`, `nUnit`, and `msTest`.
- Before-and-after regeneration scenarios for `Skip`, `Update`, and `Replace`.
- Examples with `regeneration.labels` enabled and disabled.
- Checked-in generated test projects so behavior can be reviewed without running
  the tool first.
- A nested source fixture that verifies generated tests preserve source-relative
  folders under `UnitTests/<SourceProjectName>UnitTests/...`.
- Drift automation that regenerates examples with Alchemist, builds them, and
  fails when generated output differs from the checked-in state.

## Structure

```text
Examples/
  xUnit/
    SkipLabelsOn/
    SkipLabelsOff/
    UpdateLabelsOn/
    UpdateLabelsOff/
    ReplaceLabelsOn/
    ReplaceLabelsOff/
  nUnit/
    SkipLabelsOn/
    SkipLabelsOff/
    UpdateLabelsOn/
    UpdateLabelsOff/
    ReplaceLabelsOn/
    ReplaceLabelsOff/
  msTest/
    SkipLabelsOn/
    SkipLabelsOff/
    UpdateLabelsOn/
    UpdateLabelsOff/
    ReplaceLabelsOn/
    ReplaceLabelsOff/
```

Each leaf example is a complete C# solution.

- `Example/` contains the source project that alchemist inspects.
- `UnitTests/` contains the generated unit test project.
- `README.md` records the command and expected behavior for that scenario.
- `Example/Operations/Multiplier.cs` verifies source-relative folder
  preservation by generating
  `UnitTests/ExampleUnitTests/Operations/MultiplierUnitTests.cs`.

## Example Matrix

| Framework | Skip | Update | Replace |
|-----------| --- | --- | --- |
| `xUnit`   | labels on/off | labels on/off | labels on/off |
| `nUnit`   | labels on/off | labels on/off | labels on/off |
| `msTest`  | labels on/off | labels on/off | labels on/off |

The examples all start with generated `Add_UnitTestPlaceholder` and
`Multiply_UnitTestPlaceholder` methods. The source is then expanded with
`Subtract` and `Divide` methods and Alchemist is run again. That final
checked-in state shows how each regeneration mode handles existing generated
methods, manual helper methods, new methods and nested source-relative output.

## How to Read the Examples

Start with one framework and compare the three regeneration modes.

1. `SkipLabelsOn` or `SkipLabelsOff`: Existing generated methods are kept and
   missing methods are appended.
2. `UpdateLabelsOn` or `UpdateLabelsOff`: Matching generated methods are
   refreshed, manual helpers are preserved and missing methods are appended.
3. `ReplaceLabelsOn` or `ReplaceLabelsOff`: The generated test file is rewritten
   from the current source shape.

Then compare the `LabelsOn` and `LabelsOff` variants to see whether generated
methods include `UnitTestID` comments or method-name matching.

## Regenerating Examples

Run:

```bash
scripts/RegenerateExamples.sh
```

The script generates all scenarios in a staging location and replaces
`Examples/` after the alchemist invocation succeeds. By default it downloads
the latest available `KuBuCo.Alchemist` package into a temporary tool path and
uses that executable for regeneration.

To run the same logic used by CI:

```bash
scripts/RunAlchemistDriftCheck.sh
```

Useful environment variables:

- `ALCHEMIST_VERSION=1.2.3` installs a specific downloadable
  `KuBuCo.Alchemist` tool version when validating an explicit package.
- `ALCHEMIST_PACKAGE_SOURCE=https://...` adds an extra NuGet source before
  installing the tool.
- `ALCHEMIST_PACKAGE_SOURCE_USERNAME` and `ALCHEMIST_PACKAGE_SOURCE_TOKEN`
  provide credentials for a private package source.

The drift check installs Alchemist from downloadable package sources,
regenerates all examples, restores and builds `AlchemistMetrics.sln`, restores
and builds each example solution, builds each generated
`UnitTests/UnitTests.csproj`, and finally runs `git diff --exit-code`.

## GitHub Actions

`.github/workflows/alchemist-generated-output.yml` runs the drift check:

- manually through `workflow_dispatch`;
- daily on a schedule against the latest stable downloadable
  `KuBuCo.Alchemist`;
- manually against the latest downloadable package, or against an explicit
  downloadable `alchemist_version` and `package_source` when validating a
  package feed.

For cross-repository dispatch from the main `alchemist` repository, trigger this
workflow with the target package version and a NuGet feed containing the
just-built package. A private feed requires the `alchemist-metrics` repository to
define `ALCHEMIST_PACKAGE_SOURCE_TOKEN` with read access to that feed. For GitHub
Packages, use a fine-grained PAT or equivalent token that can read packages from
the publishing repository; the default `GITHUB_TOKEN` from the caller repository
does not automatically grant package access in this repository.

## Benchmarks

The end-to-end benchmark runner measures one generated solution at a selected
size, unit test framework and regeneration mode:

```bash
scripts/RunBenchmarks.sh --profile small
```

See [`Benchmarks/README.md`](Benchmarks/README.md) for the profile sizes and
runner options.
