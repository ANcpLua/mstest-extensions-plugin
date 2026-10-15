# MSTest extensions skill

A portable plugin containing one skill for implementing MSTest data providers, combinatorial inputs,
assertions, execution attributes, conditions/retries, fixtures and test filters.
It routes runner, hosting and source-generation work to the relevant APIs.

Start at [SKILL.md](skills/mstest-extensions/SKILL.md).

## Staying current

The skill resolves the **consumer's installed version on every task**. Its
[inspector](skills/mstest-extensions/scripts/inspect_mstest.py) evaluates MSBuild,
checks restored package requests, and records framework/adapter/MTP versions,
SDK, target, source commits and assembly fingerprints. A changed version or
fingerprint requires fresh API evidence before reusing guidance. This works for
versions beyond those listed in the repository.

```sh
python3 skills/mstest-extensions/scripts/inspect_mstest.py --project /path/to/tests.csproj
# Refresh restore assets when appropriate for implementation:
python3 skills/mstest-extensions/scripts/inspect_mstest.py --project /path/to/tests.csproj --restore
```

Use repeated `--property NAME=VALUE` for configuration/target/RID and `--previous`
with an earlier JSON report to detect changes. The inspector returns exit 2 for
missing or stale evidence. It does not automatically upgrade packages or rewrite
instructions. An unchanged direct pin does not establish fresh transitive or
floating resolution; restore when needed. XML documents help locate APIs;
version-matched source and compilation establish their public contract.

[Version resolution](skills/mstest-extensions/references/version-resolution.md)
explains the workflow. [Known version notes](skills/mstest-extensions/references/version-notes.md)
record the 4.4.1 → 4.5.1 delta, including argument display labels, optional
hosting, built-in diagnostics and MTP changes. These are dated evidence, not a
hardcoded support ceiling or guarantees for future releases.

## Installation

The preferred ChatGPT/Codex installation is the **MSTest Extensions** plugin,
which packages the skill and its references without an MCP server or external
service dependency. Its skill permits automatic invocation for relevant MSTest
code work. Selection is still the agent's decision; configuration is not a
measured triggering guarantee.

Build an account-upload archive with:

```sh
python3 scripts/package_plugin.py --output /tmp/mstest-extensions.zip
```

The archive contains `plugin.json` and `skills/mstest-extensions` only. Upload it
through Plugin Creator, then install the returned plugin in the intended account.
After confirming installation, remove a duplicate standalone installation of
the same skill from that host's discovery directory. Keep one active source.

For clients using standalone skill directories, the following remains an
alternative installation path:

Python 3.10+ and directory symlink support are required for the helper scripts.
The skill's prose can also be copied into another Agent Skills-compatible client.

```sh
python3 scripts/install.py
# Also install for Claude Code:
python3 scripts/install.py --claude
```

The default creates `~/.agents/skills/mstest-extensions` for Codex. `--claude`
also links `~/.claude/skills/mstest-extensions`. Existing unrelated destinations
are preserved. Use `--agents-dir` / `--claude-dir` for custom discovery roots.

Invoke `$mstest-extensions` with, for example: “Create a reusable MSTest data
attribute and custom assertion for these scenarios, including rejection tests.”

## Verification

The checked-in sample SDK is 4.5.1, with .NET SDK 10.0.401 and MTP 2.5.1.
The verifier copies the examples into temporary workspaces and selects an exact
MSTest.Sdk version through each copy's global.json. It checks resolved versions,
discovery, behavior, original failure reports, changed API fingerprints and
reuse of unchanged evidence. Consumer projects are not modified.

```sh
python3 scripts/verify_versions.py 4.4.1 4.5.1
python3 scripts/test_inspect.py
python3 scripts/test_install.py
python3 scripts/check_links.py
```

Pass another exact SDK version to the same verifier when updating. CI runs the
recorded versions on pushes and supports a manually selected version. It does
not claim compatibility until that version's checks pass.

| SDK / framework / adapter | MTP | Passing examples | Verified runner probes |
| --- | --- | ---: | ---: |
| 4.4.1 | 2.4.1 | 31 | 5 |
| 4.5.1 | 2.5.1 | 33 | 5 |

The two additional 4.5.1 cases verify inline and dynamic `ArgumentsDisplayName`
through discovered names and executed arguments. Their source is conditionally
compiled because 4.4.1 lacks those APIs. Shared examples cover custom data,
assertion rejection, conditions, execution attributes, diagnostic failure,
async suspension/cancellation and per-row resource isolation. Recommended MSTest
analyzers remain enabled and warnings are errors.

A real child-runner discovery check verifies file/line navigation for declared
method attributes and tests with a class-created execution wrapper. Navigation
comes from the original declared attribute; the wrapper factory copies no caller
metadata and requires no `ExplicitCallerInfoArgument` suppression. The failure
probe project references the example assembly, exercising the same extension
implementation rather than compiling another copy.

For JetBrains inspections after tests have compiled the projects:

```sh
jb inspectcode MSTestExtensions.slnx --no-build --severity=SUGGESTION --format=Xml --output=artifacts/inspections.xml --properties=Configuration=Release --no-updates
```

The solution includes the intentionally failing probe executable; use the
verifiers above for execution rather than treating a whole-solution test
failure as an unexpected regression.

The probe executable deliberately returns exit 2: three failures must preserve
their original messages, one result is inconclusive, and one condition skips its
body. The verifier checks every outcome in TRX. Logs and reports stay in ignored
`artifacts/`; raw local paths, package caches and test outputs are not installed
or published with the skill.

These checks cover the examples under the JIT. They do not establish optional
hosting, Windows UI, Native AOT, every custom retry/filter implementation, model
answer quality, or automatic skill selection.

## Sources

The initial examples were authored using the supplied MSTest 4.4.1 capability
suite. The [4.5.1 API notes](https://github.com/ANcpLua/MSTest-4.4.1/tree/scout/mstest-4.5.1/delta/4.5.1)
guided the update; the sample suite on that branch still pins 4.4.1, so published
4.5.1 packages were resolved and tested separately. Framework source is
[microsoft/testfx](https://github.com/microsoft/testfx), with per-package commits
recorded in the version notes. Portable [source routes](skills/mstest-extensions/references/sources.md)
always begin with the consumer's resolved API.
