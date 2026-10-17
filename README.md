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

## Current maintenance

Version 1.2.1 integrates six upstream review topics into the existing skill:
smell detection, practical anti-pattern review, MSTest writing, test-value
evidence, assertion quality and gap analysis. The
[quality reference](skills/mstest-extensions/references/test-quality.md) separates
static findings from executed proof and avoids assertion quotas, unnecessary
seams and blanket objections to shared fixtures or integration workflows.
The [telemetry reference](skills/mstest-extensions/references/telemetry-contracts.md)
covers capture, ingestion, persistence and read-time privacy; native span
ownership; independent rows with shared publication; and artifact-specific
Native AOT evidence. It applies the captured-field phase lessons without
turning project-specific packages into universal dependencies.

Existing native cancellation, version-resolved APIs, original-result
preservation and resource ownership guidance remains in place. Required
preconditions must be asserted, structured values must retain their types, and
typed enum selectors remain distinct from shared string constants.
The verified 4.5.1/MTP 2.5.1 guidance now makes cross-cutting execution scopes,
sequential versus concurrent work, shared versus per-invocation cancellation,
and generated-discovery versus executed Native AOT evidence explicit.

Checkpoint (2026-10-17): private account plugin 1.2.1 is saved and read back.
All 27 uploaded files match source, all 28 remote paths are retained, and the
generated compatibility manifest is synchronized. Package integrity, JSON/YAML
metadata and 61 local Markdown links pass.
Existing 4.4.1/4.5.1 execution and zero-finding inspection results remain valid:
this release changes guidance and metadata, not executable examples. A
current-host reload and fresh model-selection evaluation are unverified.

The repository-local facts live in [AGENTS.md](AGENTS.md). Validation commands and
reviewed evidence remain below. Plugin model-selection evaluations are separate
historical evidence; this update does not request or rerun them.

## Installation

The preferred ChatGPT/Codex installation is the **MSTest Extensions** plugin,
which packages the skill and its references without an MCP server or external
service dependency. Version 1.2.1 preserves automatic invocation for relevant
MSTest code work. [Fresh-chat evaluations](evals/automatic-discovery/README.md),
including a [run with memory disabled](evals/automatic-discovery/without-memory.md),
record actual selection and navigation behavior separately from the example
suite. Selection remains the agent's decision on each request.

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
metadata and requires no `ExplicitCallerInfoArgument` suppression. On 4.5.1,
the internal executor declares its experimental dependency and reads the native
invocation token; the existing rows compare it with constructor-injected context. The failure
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

The plugin also packages an isolated capability verifier and its fixtures:

```sh
python3 skills/mstest-extensions/scripts/verify_capabilities.py --mstest-sdk 4.5.1 --hosting --native-aot
```

It verifies seven discovered runner-policy cases across six real child runs:
run/skip/drop, CLI selection before the provider, a throwing filter, retry
recovery/exhaustion and cooperative cancellation joined before cleanup. Three
hosted case identities use four distinct scopes across two data rows and a retry;
async disposal follows cleanup, and every controller/worker host is disposed.
The managed and osx-arm64 Native AOT published consumers each execute six cases:
five pass and one deliberate typed-value rejection preserves its original
failure. Generated invocation, original discovery locations, exact Int64 data
arguments and async lifecycle ordering are checked independently. The full
[evidence contract](skills/mstest-extensions/references/executable-evidence.md)
keeps versions, entrypoints and limits separate.

These checks cover the stated fixtures. They do not establish optional hosting
under AOT, Windows UI, browser execution, arbitrary custom attributes or every
retry/filter policy. The direct executable checks do not claim to fix the
separate official MSBuild/TRX controller limitation. Fresh-chat evaluations above
record individual 1.0.1 installed-host requests; they do not establish model
quality or trigger reliability for 1.1.2 across models, prompts or hosts.

## Sources

The initial examples were authored using the supplied MSTest 4.4.1 capability
suite. The [4.5.1 API notes](https://github.com/ANcpLua/MSTest-4.4.1/tree/scout/mstest-4.5.1/delta/4.5.1)
guided the update; the sample suite on that branch still pins 4.4.1, so published
4.5.1 packages were resolved and tested separately. Framework source is
[microsoft/testfx](https://github.com/microsoft/testfx), with per-package commits
recorded in the version notes. Portable [source routes](skills/mstest-extensions/references/sources.md)
always begin with the consumer's resolved API.
