# framework = xUnit | regeneration.mode = Skip | labels = true

```bash
alchemist --solution ./Example.sln --framework xUnit --regeneration.mode Skip --regeneration.labels true
```

## Behaviour

- `framework xUnit` write the unit tests using the xUnit framework.
- `regeneration.mode Skip` keeps existing generated tests and appends missing generated tests.
- `regeneration.labels true` emits `UnitTestID` comments on generated methods.

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the result.
