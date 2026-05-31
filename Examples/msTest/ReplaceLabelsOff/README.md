# framework = msTest | regeneration.mode = Replace | labels = false

```bash
alchemist --solution ./Example.sln --framework msTest --regeneration.mode Replace --regeneration.labels false
```

## Behaviour

- `framework msTest` write the unit tests using the microsoft framework.
- `regeneration.mode Replace` overwrites the generated test file from the current source shape.
- `regeneration.labels false` omits `UnitTestID` comments and relies on method names for update matching.

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the result.