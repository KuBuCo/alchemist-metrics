# NUnit / Update / labels=false

This example demonstrates:

- Framework: `NUnit`
- Regeneration mode: `Update`
- Regeneration labels: `false`

Command used:

```bash
alchemist --solution ./Example.sln --framework NUnit --regeneration.mode Update --regeneration.labels false
```

Inspect these generated files to see the final generated result:

- `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs`
- `UnitTests/ExampleUnitTests/Operations/MultiplierUnitTests.cs`

Expected behavior:

- `Skip` keeps the existing generated `Add_UnitTestPlaceholder` method body and appends the missing `Subtract_UnitTestPlaceholder` method.
- `Update` regenerates the matching `Add_UnitTestPlaceholder` method, preserves `ManualHelper`, and appends `Subtract_UnitTestPlaceholder`.
- `Replace` overwrites the generated test file from the current source shape.
- Generated test files preserve the source-relative folder structure, so `Example/Operations/Multiplier.cs` generates `UnitTests/ExampleUnitTests/Operations/MultiplierUnitTests.cs`.
- `labels=true` emits `UnitTestID` comments on generated methods.
- `labels=false` omits `UnitTestID` comments and relies on method names for update matching.
