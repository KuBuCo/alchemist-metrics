# framework = msTest | regeneration.mode = Update | labels = false

```bash
alchemist --solution ./Example.sln --framework msTest --regeneration.mode Update --regeneration.labels false
```

## Behaviour

- `framework msTest` write the unit tests using the microsoft framework.
- `regeneration.mode Update` regenerates matching generated tests, preserves manual tests, and appends missing generated tests.
- `regeneration.labels false` omits `UnitTestID` comments and relies on method names for update matching.

Inspect `UnitTests/ExampleUnitTests/CalculatorUnitTests.cs` to see the result.
