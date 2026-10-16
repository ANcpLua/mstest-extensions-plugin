"""Package the skill and its executable fixtures, excluding local outputs."""
import argparse
import json
from pathlib import Path
import zipfile


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, required=True, help="ZIP path outside the packaged skill directory")
    args = parser.parse_args()
    root = Path(__file__).resolve().parents[1]
    manifest = json.loads((root / "plugin.json").read_text())
    if manifest["$schema"] != "https://agent-plugins.org/schemas/1.0.0/plugin.schema.json":
        raise SystemExit("Unrecognized portable manifest schema")
    interface = manifest["extensions"]["com.openai"]["interface"]
    if len(interface["shortDescription"]) > 30:
        raise SystemExit("Plugin subtitle exceeds 30 characters")
    skill = root / "skills" / manifest["name"]
    if not (skill / "SKILL.md").is_file():
        raise SystemExit("The plugin skill is missing")
    output = args.output.resolve()
    if output.is_relative_to(skill):
        raise SystemExit("Write the archive outside the skill directory")
    files = [root / "plugin.json"]
    for path in sorted(skill.rglob("*")):
        if path.is_symlink():
            raise SystemExit(f"Symlinks cannot be packaged: {path}")
        if path.is_file() and not {"__pycache__", "bin", "obj", "artifacts"}.intersection(path.parts) and path.suffix in {".md", ".yaml", ".py", ".cs", ".csproj", ".props", ".json"}:
            files.append(path)
    output.parent.mkdir(parents=True, exist_ok=True)
    with zipfile.ZipFile(output, "w", zipfile.ZIP_DEFLATED) as archive:
        for path in files:
            archive.write(path, path.relative_to(root))
    with zipfile.ZipFile(output) as archive:
        if archive.testzip() is not None:
            raise SystemExit("Archive integrity check failed")
        for path in files:
            if archive.read(path.relative_to(root).as_posix()) != path.read_bytes():
                raise SystemExit(f"Archive content mismatch: {path}")
    print(f"Packaged {len(files)} files: {output}")


if __name__ == "__main__":
    main()
