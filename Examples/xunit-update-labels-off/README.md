# xUnit / Update / labels=false

This example demonstrates:

- Framework: `xUnit`
- Regeneration mode: `Update`
- Regeneration labels: `false`

Command used:

```bash
alchemist --solution ./Demo.sln --framework xUnit --regeneration.mode Update --regeneration.labels false
```

Inspect `UnitTests/DemoAppUnitTests/CalculatorUnitTests.cs` to see the final generated result.

Expected behavior:

- `Skip` keeps the existing generated `Add_UnitTestPlaceholder` method body and appends the missing `Subtract_UnitTestPlaceholder` method.
- `Update` regenerates the matching `Add_UnitTestPlaceholder` method, preserves `ManualHelper`, and appends `Subtract_UnitTestPlaceholder`.
- `Replace` overwrites the generated test file from the current source shape.
- `labels=true` emits `UnitTestID` comments on generated methods.
- `labels=false` omits `UnitTestID` comments and relies on method names for update matching.
