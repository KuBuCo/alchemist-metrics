# framework = msTest | regeneration.mode = Update | labels = true

```bash
alchemist --solution ./Example.sln --framework msTest --regeneration.mode Update --regeneration.labels true
```

## Behaviour

- `framework msTest` write the unit tests using the microsoft framework.
- `regeneration.mode Update` regenerates matching generated tests, preserves manual tests, and appends missing generated tests.
- `regeneration.labels true` emits `UnitTestID` comments on generated methods.

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the result.
