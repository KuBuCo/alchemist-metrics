# MSTest / Update / labels=false

This example demonstrates:

- Framework: `MSTest`
- Regeneration mode: `Update`
- Regeneration labels: `false`

Command used:

```bash
alchemist --solution ./Example.sln --framework MSTest --regeneration.mode Update --regeneration.labels false
```

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the final generated result.

Expected behavior:

- `Skip` keeps the existing generated `Add_UnitTestPlaceholder` method body and appends the missing `Subtract_UnitTestPlaceholder` method.
- `Update` regenerates the matching `Add_UnitTestPlaceholder` method, preserves `ManualHelper`, and appends `Subtract_UnitTestPlaceholder`.
- `Replace` overwrites the generated test file from the current source shape.
- `labels=true` emits `UnitTestID` comments on generated methods.
- `labels=false` omits `UnitTestID` comments and relies on method names for update matching.
