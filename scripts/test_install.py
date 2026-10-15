import tempfile
import unittest
from pathlib import Path
from install import install


class InstallTests(unittest.TestCase):
    def test_second_install_keeps_both_links_and_source_unchanged(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            source = root / "source"
            source.mkdir()
            (source / "SKILL.md").write_text("owned source")
            destinations = [root / "agents" / "skill", root / "claude" / "skill"]
            install(source, destinations)
            before = [destination.lstat().st_mtime_ns for destination in destinations]
            install(source, destinations)
            self.assertEqual(before, [destination.lstat().st_mtime_ns for destination in destinations])
            self.assertTrue(all(destination.resolve() == source.resolve() for destination in destinations))
            self.assertEqual("owned source", (source / "SKILL.md").read_text())

    def test_conflict_is_detected_before_creating_any_link(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            source = root / "source"
            source.mkdir()
            first = root / "agents" / "skill"
            occupied = root / "claude" / "skill"
            occupied.mkdir(parents=True)
            (occupied / "user.txt").write_text("keep")
            with self.assertRaises(FileExistsError):
                install(source, [first, occupied])
            self.assertFalse(first.exists())
            self.assertEqual("keep", (occupied / "user.txt").read_text())


if __name__ == "__main__":
    unittest.main()
