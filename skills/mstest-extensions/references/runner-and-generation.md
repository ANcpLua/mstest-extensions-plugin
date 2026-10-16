# Runner, packaging, and source generation

Separate three extension layers:

1. MSTest framework extensions: attributes, data sources, assertions, fixtures,
   and filters consumed by the MSTest adapter.
2. MTP extensions: reporting, host services, command-line options, and process
   integration using Microsoft.Testing.Platform APIs and builder registration.
3. Compile-time discovery generation: MSTest's generator and its supported test
   shapes, including Native AOT scenarios.

A custom assertion does not need an MTP plugin. A runtime data provider does not
need a Roslyn source generator. Create an MTP extension only when the requested
behavior belongs to the host, rather than an individual MSTest invocation. Match
the platform package version and inspect its extension interfaces and registration
mechanism before implementing one.

## Use the consumer's runner

Identify the runner from project/SDK imports, resolved package assets,
`global.json`, and the repository's normal test command. `MSTest.Sdk` can configure
the runner and optional extensions; an existing `Microsoft.NET.Sdk` project can
use explicit framework/adapter packages. Preserve whichever shape the consumer
uses unless migration is requested.

MTP and VSTest do not accept identical arguments or configuration. With .NET 10
MTP mode selected in `global.json`, project selection uses `dotnet test
--project ...`; use the chosen runner's help to resolve discovery, filtering,
minimum test counts, reporting, and coverage flags. Do not mix VSTest's
`--collect` or logger syntax with an MTP extension's options. Enable only the
reporting/coverage extensions the task needs, preserving existing providers.

Discovery must find the new cases, but is not execution. Verify passing tests,
and use a separate process for expected failures. Inspect structured reports for
the expected case identities, outcomes, and original failure messages, as well
as the exit status. A build error, zero tests, or unrelated crash is not proof
that an assertion or wrapper correctly rejected a case.

MSTest 4.4's MTP integration no longer uses VSTestBridge. Do not reintroduce that
bridge based on old examples. Match CLI, adapter, and extension versions when a
consumer fails before executing its test body.

## Source generation and Native AOT

`EnableMSTestSourceGeneration` is an SDK option, separate from custom data
attributes. In the inspected SDKs it is opt-in for ordinary projects and enabled
for Native AOT. Recheck SDK defaults when its version changes. Inspect generated files and `AOTSG` diagnostics
when the request involves discovery generation.

Generated metadata, direct invocation, fallback paths, and reflection-free
Native AOT support are different claims. Async tests, custom method attributes,
dynamic data, and other complex shapes can take fallback paths; do not infer
that an extension works under Native AOT merely because it builds or runs under
the JIT with source generation enabled. Generic tests, inaccessible types,
inheritance, provider discovery, and reflection-dependent plugins need their own
compatibility evidence.

For an AOT claim, publish for the actual target RID and execute the published
artifact. A green JIT run is not that evidence. The packaged
[published-consumer verifier](executable-evidence.md) keeps generated sources,
discovery locations, full reports and the executed binary hash. Its direct
invocation assertion distinguishes generated execution from reflection fallback. Preserve generator warnings
unless a specific limitation is understood and explicitly scoped; example
projects demonstrating diagnostics are not templates for blanket suppression.

Optional hosting, configuration and orchestration APIs belong to the resolved
MTP extension packages. Their versions and experimental status need separate
checks; a framework update does not make them automatically available. Use
[known release notes](version-notes.md) only as a starting index for these APIs.

## Reusable packaging

Keep framework extension libraries separate from test executables and host
plugins. Choose target frameworks from the consumers, keep package pins coherent,
and include an actual consuming test project to verify registration/discovery.
Do not use a local developer's solution path as a runtime dependency. Publishing
a reusable package is a separate task from implementing its source.

Sources: [SDK configuration](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-sdk),
[runner documentation](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-runner-intro),
and the installed SDK imports described in [source routes](sources.md).
