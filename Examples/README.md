# Examples

Each directory is a complete C# solution plus the generated `UnitTests` output
from Alchemist.

Directory names use this format:

```text
<framework>-<regeneration-mode>-<labels-on|labels-off>
```

For example, `xunit-update-labels-on` shows the output from:

```bash
alchemist --solution ./Demo.sln --framework xUnit --regeneration.mode Update --regeneration.labels true
```

The matrix contains:

| Framework | Skip | Update | Replace |
| --- | --- | --- | --- |
| xUnit | labels on/off | labels on/off | labels on/off |
| NUnit | labels on/off | labels on/off | labels on/off |
| MSTest | labels on/off | labels on/off | labels on/off |

Inside each example:

- `DemoApp/` is the source project Alchemist inspects.
- `UnitTests/` is the generated result.
- `initial-run.txt` captures the seed generation run.
- `final-run.txt` captures the run for the directory's option combination.
- `README.md` states the exact command and behavior to inspect.
