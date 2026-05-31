# framework = nUnit | regeneration.mode = Update | labels = true

```bash
alchemist --solution ./Example.sln --framework nUnit --regeneration.mode Update --regeneration.labels true
```

## Behaviour

- `framework nUnit` write the unit tests using the nUnit framework.
- `regeneration.mode Update` regenerates matching generated tests, preserves manual tests, and appends missing generated tests.
- `regeneration.labels true` emits `UnitTestID` comments on generated methods.

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the result.
