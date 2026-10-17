# Changelog

## 3.2.0 — 2026-10-17

- Distinguish unit, integration, end-to-end and shared-helper scopes inside the
  existing plugin, with native MSTest composition and resource ownership.
- Prefer native execution/context and MSBuild metadata over reflection-based
  discovery or configuration lookup; retain public data-provider metadata contracts.
- Verify original declaration locations through the real runner, removing the
  reflective C# navigation harness and redundant direct provider test.
- Document source-glob, obsolete-API and custom reporter pitfalls observed in
  the 3.0.1 evaluation. Preserve its initial activation miss and corrected results.
- Remove obsolete local artifacts after retaining the useful evaluation record.

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
