#!/usr/bin/env python3

import hashlib
import json
import sys
from pathlib import Path


REPOSITORY_ROOT = Path(__file__).resolve().parent.parent
EXAMPLES_ROOT = REPOSITORY_ROOT / "Examples"
FRAMEWORKS = {
    "xUnit": "[Fact",
    "nUnit": "[Test, Ignore",
    "msTest": "[TestMethod, Ignore",
}
MODES = ("Skip", "Update", "Replace")
LABEL_STATES = ("LabelsOn", "LabelsOff")
GENERATED_FILES = {
    "ExampleUnitTests/CalculatorUnitTests.cs": (
        "Add_UnitTestPlaceholder",
        "Subtract_UnitTestPlaceholder",
    ),
    "ExampleUnitTests/Construction/PrimitiveSubjectUnitTests.cs": (
        "Measure_UnitTestPlaceholder",
    ),
    "ExampleUnitTests/Operations/MultiplierUnitTests.cs": (
        "Multiply_UnitTestPlaceholder",
        "Divide_UnitTestPlaceholder",
    ),
}


def fail(message: str) -> None:
    print(message, file=sys.stderr)
    raise SystemExit(1)


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest().upper()


def validate_manifest(example: Path) -> None:
    manifest_path = example / "UnitTests/.alchemist-generated-files.json"
    try:
        manifest = json.loads(manifest_path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as error:
        fail(f"Invalid ownership manifest {manifest_path}: {error}")

    if manifest.get("SchemaVersion") != 1:
        fail(f"Unsupported ownership manifest schema in {manifest_path}.")

    files = manifest.get("Files")
    if not isinstance(files, list) or len(files) != len(GENERATED_FILES):
        fail(f"Unexpected ownership entries in {manifest_path}.")

    expected_paths = {f"UnitTests/{path}" for path in GENERATED_FILES}
    actual_paths = {entry.get("Path") for entry in files if isinstance(entry, dict)}
    if actual_paths != expected_paths:
        fail(f"Ownership paths do not match generated files in {manifest_path}.")

    for entry in files:
        relative_path = entry.get("Path")
        content_hash = entry.get("ContentHash")
        if not isinstance(relative_path, str) or not isinstance(content_hash, str):
            fail(f"Malformed ownership entry in {manifest_path}.")

        generated_path = example / relative_path
        if not generated_path.is_file():
            fail(f"Owned generated file does not exist: {generated_path}")
        if len(content_hash) != 64 or content_hash.upper() != sha256(generated_path):
            fail(f"Ownership hash does not match {generated_path}.")


def validate_example(framework: str, mode: str, label_state: str) -> None:
    scenario = f"{mode}{label_state}"
    example = EXAMPLES_ROOT / framework / scenario
    if not example.is_dir():
        fail(f"Missing example scenario: {example}")

    required_paths = (
        example / "Example.sln",
        example / "Example/Example.csproj",
        example / "UnitTests/UnitTests.csproj",
        example / "README.md",
    )
    for path in required_paths:
        if not path.is_file():
            fail(f"Missing required example file: {path}")

    solution = (example / "Example.sln").read_text(encoding="utf-8-sig")
    if "UnitTests\\UnitTests.csproj" not in solution:
        fail(f"Generated test project is not registered in {example / 'Example.sln'}.")

    generated_text = ""
    placeholder_count = 0
    for relative_path, placeholders in GENERATED_FILES.items():
        path = example / "UnitTests" / relative_path
        if not path.is_file():
            fail(f"Missing generated file: {path}")

        text = path.read_text(encoding="utf-8-sig")
        generated_text += text
        for placeholder in placeholders:
            if text.count(placeholder) != 1:
                fail(f"Expected one {placeholder} method in {path}.")
            placeholder_count += 1

    expected_attribute = FRAMEWORKS[framework]
    if generated_text.count(expected_attribute) != placeholder_count:
        fail(f"Unexpected test attributes in {example}.")

    label_count = generated_text.count("// [UnitTestID=")
    expected_label_count = placeholder_count if label_state == "LabelsOn" else 0
    if label_count != expected_label_count:
        fail(f"Unexpected regeneration-label count in {example}.")

    calculator = (
        example / "UnitTests/ExampleUnitTests/CalculatorUnitTests.cs"
    ).read_text(encoding="utf-8-sig")
    preserves_manual_members = mode in ("Skip", "Update")
    if ("ManualHelper" in calculator) != preserves_manual_members:
        fail(f"Unexpected manual-helper preservation behavior in {example}.")
    if ("manual-edit-preserved" in calculator) != (mode == "Skip"):
        fail(f"Unexpected generated-method replacement behavior in {example}.")

    primitive = (
        example
        / "UnitTests/ExampleUnitTests/Construction/PrimitiveSubjectUnitTests.cs"
    ).read_text(encoding="utf-8-sig")
    if "new PrimitiveSubject(default)" not in primitive or "Mock<int>" in primitive:
        fail(f"Primitive constructor contract failed in {example}.")

    validate_manifest(example)


def main() -> None:
    if not EXAMPLES_ROOT.is_dir():
        fail(f"Missing examples directory: {EXAMPLES_ROOT}")

    expected_scenarios = {
        framework: {f"{mode}{labels}" for mode in MODES for labels in LABEL_STATES}
        for framework in FRAMEWORKS
    }
    actual_frameworks = {path.name for path in EXAMPLES_ROOT.iterdir() if path.is_dir()}
    if actual_frameworks != set(FRAMEWORKS):
        fail("The example framework matrix is incomplete or contains unexpected directories.")

    for framework, scenarios in expected_scenarios.items():
        framework_root = EXAMPLES_ROOT / framework
        actual_scenarios = {path.name for path in framework_root.iterdir() if path.is_dir()}
        if actual_scenarios != scenarios:
            fail(f"The scenario matrix is incomplete under {framework_root}.")

        for mode in MODES:
            for label_state in LABEL_STATES:
                validate_example(framework, mode, label_state)

    print("Validated all 18 generated-output scenarios and ownership manifests.")


if __name__ == "__main__":
    main()
