# framework = nUnit | regeneration.mode = Skip | labels = true

```bash
alchemist --solution ./Example.sln --framework nUnit --regeneration.mode Skip --regeneration.labels true
```

## Behaviour

- `framework nUnit` write the unit tests using the nUnit framework.
- `regeneration.mode Skip` keeps existing generated tests and appends missing generated tests.
- `regeneration.labels true` emits `UnitTestID` comments on generated methods.

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the result.
