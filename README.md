# Alchemist Demo

Public examples for the `KuBuCo.Alchemist` .NET tool.

This repository is intentionally small and public-facing. It shows the source
solutions Alchemist can inspect and the generated `UnitTests` output it creates
for each supported framework, regeneration mode, and label setting.

## Contents

- `Examples/`: one self-contained solution per option combination.
- `scripts/regenerate-examples.sh`: rebuilds the examples with an installed
  `alchemist` command.

## Matrix

The examples cover:

- Frameworks: `xUnit`, `NUnit`, `MSTest`
- Regeneration modes: `Skip`, `Update`, `Replace`
- Regeneration labels: `true`, `false`

That produces 18 example solutions.

Each example starts from a simple source project with an existing generated test
file, then reruns Alchemist after the source project gains another public
method. The final checked-in state demonstrates how the selected mode handles
existing generated methods, manual helper methods, and newly discovered source
methods.

## Regenerating

Install Alchemist as a .NET tool, then run:

```bash
scripts/regenerate-examples.sh
```

To use a locally built command wrapper instead of the global tool:

```bash
ALCHEMIST_COMMAND=/path/to/alchemist scripts/regenerate-examples.sh
```

The script only requires an `alchemist` command on the local machine.
