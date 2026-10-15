"""Link the MSTest skill into local Codex and optionally Claude Code discovery."""
import argparse
from pathlib import Path


def install(source, destinations):
    source = source.resolve(strict=True)
    for destination in destinations:
        if destination.exists() or destination.is_symlink():
            if not destination.is_symlink() or destination.resolve() != source:
                raise FileExistsError(f"Refusing to replace existing skill: {destination}")
    for destination in destinations:
        if destination.is_symlink():
            print(f"Already linked: {destination}")
        else:
            destination.parent.mkdir(parents=True, exist_ok=True)
            destination.symlink_to(source, target_is_directory=True)
            print(f"Linked: {destination}")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--claude", action="store_true", help="Also install for Claude Code")
    parser.add_argument("--agents-dir", type=Path, default=Path.home() / ".agents" / "skills")
    parser.add_argument("--claude-dir", type=Path, default=Path.home() / ".claude" / "skills")
    args = parser.parse_args()
    source = Path(__file__).resolve().parents[1] / "skills" / "mstest-extensions"
    destinations = [args.agents_dir / source.name]
    if args.claude:
        destinations.append(args.claude_dir / source.name)
    install(source, destinations)


if __name__ == "__main__":
    main()
