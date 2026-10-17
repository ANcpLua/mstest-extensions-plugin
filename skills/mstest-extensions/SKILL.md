---
name: mstest-extensions
description: >-
  Create, adapt, review, or troubleshoot MSTest tests and reusable extensions:
  custom data attributes, combinatorial providers, assertions, execution
  attributes, conditions, fixtures, and test filters. Use when implementing
  testing capabilities with MSTest or migrating extensions to MSTest. Ordinary
  application changes and running an unchanged test command are outside this
  workflow.
---

# MSTest testing and extensions

Build the requested capability with the consuming project's MSTest APIs. Start
with built-in behavior; create an extension when it removes repeated complexity
or expresses a domain contract.

## Establish the API and execution contract

Read the project's instructions, package/SDK pins, target frameworks,
`global.json`, and existing tests. Identify MSTest.TestFramework, the adapter,
MSTest.Sdk if used, and whether the entrypoint runs Microsoft.Testing.Platform
(MTP) or VSTest. Preserve the consumer's versions, runner, and coverage provider
unless changing them is part of the task. Resolve the actual restored versions
for each target/configuration at the start of a task; a repository name or an
earlier conversation's version is not current evidence.

Use [version resolution](references/version-resolution.md) and the bundled
[API inspector](scripts/inspect_mstest.py) to identify evaluated package requests,
resolved framework/adapter/platform versions, SDK, source commits, and assembly
fingerprints. Changed versions, assets, targets, runner settings, or generation
mode invalidate earlier API evidence. Resolve relevant signatures from the
installed package and its matching source before reusing a reference's pattern.
The skill supports this lookup for unlisted versions; a release note entry is not
a prerequisite. [Known release notes](references/version-notes.md) are dated
evidence, not a promise about later versions.

Name the observable behavior, its public entrypoint, and the owner of each
resource. Keep explanations and reviews read-only. For implementation, choose a
case that distinguishes the required behavior from a plausible incorrect result.

Use [source routes](references/sources.md) to resolve the needed API. Prefer a
user-supplied matching solution and installed package metadata; distinguish a
consumer sample from framework implementation. Check official version-matched
source when signatures or runtime behavior remain unclear. Current documentation
can include later or preview APIs. Compile before calling a code example verified.

## Choose the extension point

Read only the reference that applies to the requested capability.

| Need | MSTest mechanism | Reference |
| --- | --- | --- |
| Inline or computed cases | `[TestMethod]` with `[DataRow]` or `[DynamicData]` | [Data](references/data.md) |
| Reusable named scenario attribute | `Attribute, ITestDataSource` | [Data](references/data.md) |
| Independent input axes or custom parameter values | `[CombinatorialData]`, `ICombinatorialValuesProvider` when available | [Data](references/data.md) |
| A repeated domain expectation | Extension method on `Assert`, called through `Assert.That` | [Assertions](references/assertions.md) |
| A policy around a test invocation | `TestMethodAttribute.ExecuteAsync`; class-wide composition through `TestClassAttribute` | [Execution](references/execution.md) |
| Conditional execution or retry policy | Built-in conditions/retries, then `ConditionBaseAttribute` or `RetryBaseAttribute` | [Execution](references/execution.md) |
| Per-test resources, shared setup, or controlled suspension | Initialization/cleanup methods, `TestContext`, explicit resource ownership | [Lifecycle](references/lifecycle.md) |
| Coordinating shared resources under parallel execution | Explicit parallel policy and `[ResourceLock]` when available | [Lifecycle](references/lifecycle.md) |
| Constructing test classes from an application-owned service provider | Installed hosting integration and its compatibility limits | [Lifecycle](references/lifecycle.md) |
| Programmatic run selection | `ITestFilter` and one assembly-level provider | [Execution](references/execution.md) |
| Discovery, reporting, MTP integration, or source generation/AOT | Existing runner and its supported extension APIs | [Runner and generation](references/runner-and-generation.md) |

## Implement with MSTest semantics

Keep data descriptions immutable and create mutable resources per execution.
Discovery and data enumeration must not acquire per-invocation resources.
Optional hosting can bootstrap a root host even for discovery; keep its startup
bounded and verify it creates no invocation scopes. Preserve every result,
original failure, data-row argument and lifecycle behavior when wrapping execution.
Let MSTest own row expansion; do not manually re-enumerate `[DataRow]` inside a
wrapper or collapse its results. Read [data guidance](references/data.md) when
composing extensions around parameterized tests.
A required verification or cleanup failure must remain visible, including when
another test failure already exists. Optional diagnostic capture has a different
failure policy. Read [execution](references/execution.md) and
[lifecycle](references/lifecycle.md) before choosing that policy.

Use published framework or domain constants for fixed contract strings. Define
shared named constants for owned keys, event names, configuration settings,
resource identifiers and diagnostic markers instead of repeating literals. Keep
typed enum selectors typed: `RuleFamily.Dependencies` is an enum member, not a
string constant. Pass it directly to a typed attribute constructor; do not replace
it with a string or `.ToString()`. Use `nameof` for member/type references. Keep distinct adversarial inputs distinct;
do not normalize test values or create a second catalog of published constants.
See [constant strings](references/execution.md#constant-strings-and-contracts).

Scope experimental API acknowledgments to the specific code that requires them.
Where supported, declare an internal wrapper's experimental dependency with the
same diagnostic ID instead of suppressing the diagnostic. Do not disable
analyzers project-wide. Native invocation context is version-dependent; see
[execution](references/execution.md#native-invocation-context-and-cancellation).
For class-created execution wrappers, source navigation comes from discovery of
the original test attribute. Use the [execution guidance](references/execution.md)
before copying caller metadata or adding an inspection suppression.

An `ITestDataSource` is a runtime data provider. MSTest source generation is a
separate runner/discovery feature. An `Assert.That` extension is an ordinary C#
extension method; it does not need an assertion generator. Synchronous MSTest
assertions throw immediately; await operations and async assertions that return
tasks. Do not transfer another framework's fixture injection, executor, or
parallel-default assumptions into MSTest.

## Verify the capability

Discover the affected tests through the consuming project's normal entrypoint;
check row names and counts, then run the focused cases. Tests already compile
the code. For a custom assertion, exercise acceptance and rejection. For data
providers, verify repeatable enumeration and meaningful combinations. For
execution policies, check a real runner failure as well as a passing invocation;
an exception swallowed into a passing result is a defect.

For resources and async work, verify per-test isolation, actual suspension,
successful resumption, cancellation, and joining before disposal. Use the
lifecycle reference when observation and cleanup can both fail. Use a separate
process for deliberately failing runner probes, and check its exit status and
structured results rather than treating a nonzero exit alone as success.

Report the commands, discovered/executed cases, and remaining limits. Keep model
content evaluations, automatic skill selection, compilation, runner execution,
and Native AOT publication as separate evidence. Repository-specific commands and
test baselines belong in the project instructions, outside this portable skill.

For executable checks of retry/filter outcomes, optional host scope disposal, or
published source-generated and Native AOT consumers, use the packaged
[capability verifier](references/executable-evidence.md). It runs isolated
fixtures with explicit versions; it does not change the consumer's packages or
establish compatibility for untested extension shapes.
