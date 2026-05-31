# framework = xUnit | regeneration.mode = Replace | labels = true

```bash
alchemist --solution ./Example.sln --framework xUnit --regeneration.mode Replace --regeneration.labels true
```

## Behaviour

- `framework xUnit` write the unit tests using the xUnit framework.
- `regeneration.mode Replace` overwrites the generated test file from the current source shape.
- `regeneration.labels true` emits `UnitTestID` comments on generated methods.

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the result.
