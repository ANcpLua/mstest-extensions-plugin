# Changelog

## 3.0.1 — 2026-10-17

- Expand skill activation to MSTest planning, explanations and API questions,
  including requests without a selected consumer project or package version.
- Keep conceptual answers read-only and distinguish unresolved API contracts
  from signatures verified against installed packages.
- Use the normal test entrypoint for changed-code compilation and restore;
  resolve assertion APIs and analyzer recommendations before writing tests.
- Retain the unchanged 3.0.0 evaluation prompts for regression comparisons.

## 3.0.0 — 2026-10-17

- Rename the repository to `ANcpLua/mstest-extensions-plugin` and update the
  manifest repository URL.
- Preserve the `mstest-extensions` plugin identifier, skill name and
  **MSTest Extensions** display name.
- Clarify the package boundary: one focused skill with 11 supporting references,
  scripts and executable fixtures. Keep the existing skill and runtime behavior.
- Publish a versioned plugin archive and record fresh evaluations separately
  from the existing compatibility and runner evidence.
