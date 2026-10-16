# Lifecycle, resources, and async isolation

Use `[TestInitialize]` and `[TestCleanup]` for per-test resources; data rows each
have their own test lifecycle. Async setup/cleanup can return `Task` or `ValueTask`
in the inspected versions. Keep constructors resource-light and roll back partial setup
inside the acquisition helper. Do not rely on teardown for an object that never
finished construction.

MSTest supports `TestContext` injection through the test-class constructor in the
inspected versions, or the supported property pattern. Obtain cooperative cancellation
from `TestContext.CancellationToken`; do not assume arbitrary test-method token
injection. Thread it into owned operations. Inspect
`CurrentTestOutcome` in cleanup for failure diagnostics and use context output or
result attachments while their data remains available.

Class/assembly initialize and cleanup methods are static and have their own
signature/context rules. In MSTest 4, class cleanup occurs at the end of the
class; do not transplant older `ClassCleanupBehavior` examples. Global test
fixtures have a wider application scope and their ordering is not a dependency
mechanism. Choose the narrowest scope that owns the resource.

For reusable assembly setup, inspect `AssemblyFixtureProviderAttribute` in the
installed adapter before choosing registration placement. In the inspected
versions, a shared library can advertise fixture methods to consuming
assemblies, while local assembly fixture declarations take precedence. Native
AOT skips this provider discovery. Verify actual initialization and cleanup in
the consuming assembly instead of assuming that referencing a fixture library
is sufficient.

## Hosting integration when available

Resolve the optional hosting package independently. In the inspected 4.5.1
release family, `AddMSTestTestClassInjection` connects a caller-owned host's
services to test-class construction, with a separate scope per invocation, row,
and retry. Ordinary test initialization/cleanup still applies. The application
owns the root host; the integration owns the invocation scope. The checked MTP
controller and worker each call the host factory, and discovery can bootstrap
a host too. Do not assume one host across all processes or allocate invocation
services during discovery. Verify each process's host lifetime and await scoped
async disposal after cleanup. The packaged [real-host probe](executable-evidence.md)
checks data-row and retry isolation with this ownership boundary.

The inspected integration is experimental and excludes source generation,
Native AOT and browser/AOT execution. Confirm current constraints and diagnostics
from the resolved package before using or rejecting it on a newer release.
See [known version evidence](version-notes.md) for the source of these limits.

## Parallel execution and ownership

Declare a deliberate `[assembly: Parallelize(...)]` or
`[assembly: DoNotParallelize]` policy consistent with the repository. MSTest does
not share TUnit's parallel-by-default contract. Give each test its own workspace,
stream, mutable fixture, or server partition. Shared fixtures need immutable
state, separate per-test partitions, or synchronization.

When supported (introduced in 4.4), `[ResourceLock("key")]` coordinates participating tests in the same
assembly. Keys are ordinal and case-sensitive; path-like keys do not establish
parent/child locks. Default access is exclusive; `ResourceAccessMode.Read` allows
concurrent readers. Every relevant reader and writer must participate. These
locks are not cross-process or cross-assembly locks. Use built-in well-known
resource names when coordinating process-wide state within the assembly.

## Actual async suspension

A completed `Task` is not a suspension test. Use a controllable dependency with
separate reached/resume signals and asynchronous continuations. Record that the
operation reached the intended point and is still pending; then resume it and
check its result. Prove cancellation in a separate case.

Always join the **original operation** before disposing its dependencies,
including when an observation/assertion fails. A `WaitAsync` timeout only ends
the wait; it does not stop that original operation. A timeout attribute also does
not make uncooperative code safe to dispose. Establish what terminates the whole
operation, not just its current stream read. Use an owned process with explicit
termination/reaping when a hard bound is required for uncooperative work.

For a finite cooperative copy, keep assertions after releasing and joining:

```csharp
var copying = source.CopyToAsync(destination, token);
bool wasPending;
try
{
    await Task.WhenAny(source.Paused, copying);
    wasPending = source.Paused.IsCompletedSuccessfully && !copying.IsCompleted;
}
finally
{
    source.Resume();
}
await copying;
Assert.IsTrue(wasPending);
```

`WhenAny` does not propagate the copy's exception; the final await does. This
observation block has no throwing assertions or independently cancelled wait.
If it gains them, move joining into cleanup and retain both observation and
cleanup failures, for example as an aggregate with the original first. Do not
let an exception from an awaited `finally` replace the original failure.

Only optional diagnostics may swallow errors. Resource cleanup and application
faults must stay visible. Check capture failure and sink failure independently;
non-blocking snapshots are preferable to promises of timeouts around arbitrary
logging.

Sources: [lifecycle](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-writing-tests-lifecycle),
[TestContext](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-writing-tests-testcontext),
and the package contracts in [source routes](sources.md).
