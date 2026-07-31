# NUnit / Skip / labels=false

This example demonstrates:

- Framework: `NUnit`
- Regeneration mode: `Skip`
- Regeneration labels: `false`

Command used:

```bash
alchemist --solution ./Example.sln --framework NUnit --regeneration.mode Skip --regeneration.labels false
```

Inspect these generated files to see the final generated result:

- `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs`
- `UnitTests/ExampleUnitTests/Construction/PrimitiveSubjectUnitTests.cs`
- `UnitTests/ExampleUnitTests/Operations/MultiplierUnitTests.cs`

Expected behavior:

- `Skip` keeps the existing generated `Add_UnitTestPlaceholder` method body and appends the missing `Subtract_UnitTestPlaceholder` method.
- `Update` regenerates the matching `Add_UnitTestPlaceholder` method, preserves `ManualHelper`, and appends `Subtract_UnitTestPlaceholder`.
- `Replace` overwrites the generated test file from the current source shape.
- Generated test files preserve the source-relative folder structure, so `Example/Operations/Multiplier.cs` generates `UnitTests/ExampleUnitTests/Operations/MultiplierUnitTests.cs`.
- Non-mockable constructor parameters use `default` arguments instead of invalid mock declarations, so `PrimitiveSubject(int count)` generates `new PrimitiveSubject(default)` without `Mock<int>`.
- `labels=true` emits `UnitTestID` comments on generated methods.
- `labels=false` omits `UnitTestID` comments and relies on method names for update matching.
