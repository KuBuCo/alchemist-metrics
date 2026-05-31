# framework = nUnit | regeneration.mode = Skip | labels = false

```bash
alchemist --solution ./Example.sln --framework nUnit --regeneration.mode Skip --regeneration.labels false
```

## Behaviour

- `framework nUnit` write the unit tests using the nUnit framework.
- `regeneration.mode Skip` keeps existing generated tests and appends missing generated tests.
- `regeneration.labels false` omits `UnitTestID` comments and relies on method names for update matching.

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the result.
