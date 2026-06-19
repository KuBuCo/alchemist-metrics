# Alchemist Demo

Companion examples for
[`KuBuCo.Alchemist`](https://kubuco.github.io/alchemist/readme/).

The public documentation is the source of truth for what the tool does, how to 
install it and how its command-line options work. This exists to make those 
options inspectable: It contains small source projects together with the 
`UnitTests` project generated for each supported test framework, regeneration 
mode and label setting.

## What This Adds

- A concrete example matrix for `xUnit`, `nUnit`, and `msTest`.
- Before-and-after regeneration scenarios for `Skip`, `Update`, and `Replace`.
- Examples with `regeneration.labels` enabled and disabled.
- Checked-in generated test projects so behavior can be reviewed without running
  the tool first.

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

## Example Matrix

| Framework | Skip | Update | Replace |
|-----------| --- | --- | --- |
| `xUnit`   | labels on/off | labels on/off | labels on/off |
| `nUnit`   | labels on/off | labels on/off | labels on/off |
| `msSTest` | labels on/off | labels on/off | labels on/off |

The examples all start with a generated `Add_UnitTestPlaceholder` method.
The source is then expanded with a `Subtract` method and Alchemist is run again.
That final checked-in state shows how each regeneration mode handles existing
generated methods, manual helper methods and new methods.

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
`Examples/` after the alchemist invocation succeeds.

## Benchmarks

The end-to-end benchmark runner measures one generated solution at a selected
size, unit test framework and regeneration mode:

```bash
scripts/RunBenchmarks.sh --profile small
```

See [`Benchmarks/README.md`](Benchmarks/README.md) for the profile sizes and
runner options.
