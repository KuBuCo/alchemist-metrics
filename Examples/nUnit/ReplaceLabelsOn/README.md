# framework = nUnit | regeneration.mode = Replace | labels = true

```bash
alchemist --solution ./Example.sln --framework nUnit --regeneration.mode Replace --regeneration.labels true
```

## Behaviour

- `framework nUnit` write the unit tests using the nUnit framework.
- `regeneration.mode Replace` overwrites the generated test file from the current source shape.
- `regeneration.labels true` emits `UnitTestID` comments on generated methods.

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the result.
