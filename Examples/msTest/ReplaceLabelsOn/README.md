# framework = msTest | regeneration.mode = Replace | labels = true

```bash
alchemist --solution ./Example.sln --framework msTest --regeneration.mode Replace --regeneration.labels true
```

## Behaviour

- `framework msTest` write the unit tests using the microsoft framework.
- `regeneration.mode Replace` overwrites the generated test file from the current source shape.
- `regeneration.labels true` emits `UnitTestID` comments on generated methods.

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the result.
