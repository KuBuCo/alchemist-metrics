# framework = xUnit | regeneration.mode = Skip | labels = false

```bash
alchemist --solution ./Example.sln --framework xUnit --regeneration.mode Skip --regeneration.labels false
```

## Behaviour

- `framework xUnit` write the unit tests using the xUnit framework.
- `regeneration.mode Skip` keeps existing generated tests and appends missing generated tests.
- `regeneration.labels false` omits `UnitTestID` comments and relies on method names for update matching.

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the result.
