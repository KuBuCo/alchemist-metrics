# Alchemist Benchmarks

These are end-to-end benchmarks for Alchemist. The runner creates a
deterministic solution, runs the `alchemist` command once, and writes
JSON/CSV reports. It never modifies `Examples/`.

## Profiles

| Profile | Projects | Methods |
| --- | ---: | ---: |
| `small` | 1 | 100 |
| `medium` | 10 | 1,000 |
| `large` | 100 | 10,000 |

`--framework` chooses the unit test framework and `--mode` chooses the
regeneration mode passed to Alchemist.

## Run

```bash
scripts/RunBenchmarks.sh --profile small
```

Use another framework or regeneration mode.

```bash
scripts/RunBenchmarks.sh --profile medium --framework nunit --mode update
```

Results are written as JSON and CSV under `Benchmarks/Results/{ddMMyyyy}/`
using the filenames `benchmark-{profile}-{timestamp}.json` and
`benchmark-{profile}-{timestamp}.csv`. Use `--keep-fixtures` to retain
solutions for diagnosis.

If `alchemist` is not on `PATH`, pass it.

```bash
scripts/RunBenchmarks.sh --alchemist /path/to/alchemist
```

Run `scripts/RunBenchmarks.sh --help` for all options.
