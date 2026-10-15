# Execution attributes and policies

## Wrap the framework invocation

For the async invocation API checked in 4.4.1 and 4.5.1, derive from
`TestMethodAttribute` and override
`Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)`. Forward caller file and
line information from the custom attribute constructor to the base constructor
when preserving source navigation. Prefer `await base.ExecuteAsync(testMethod)`
for a wrapper. If manually invoking once, use
`await testMethod.InvokeAsync(testMethod.Arguments)` and return its result.

Never invoke `MethodInfo` directly: that bypasses MSTest's invocation handling.
An assertion failure can be represented by a returned `TestResult`; a
`try/catch` around the await is not sufficient to detect failed tests. Preserve
every result's outcome, exception, attachments, duration, and identity. Do not
replace results with newly constructed passing objects.

Use `[TestInitialize]`/`[TestCleanup]` for resource lifetime. A custom invocation
attribute is not a substitute for all runner scheduling and lifecycle hooks.
Keep invocation-local state local; do not retain mutable test state in attribute
instances.

For class-wide behavior, override
`TestClassAttribute.GetTestMethodAttribute(TestMethodAttribute)`. Compose around
the provided attribute so a method's own execution semantics survive; blindly
returning a new base attribute erases them. Use built-in STA attributes when
supported and needed instead of inventing a synchronization context or executor.
Verify platform support and continuation affinity separately.

Check built-in assertion-failure diagnostics in the resolved version before
creating a capture wrapper; see [known version changes](version-notes.md).
Optional custom diagnostics can inspect results and append to `LogOutput`.
Contain failures from both capture and output so they cannot replace the test
failure. Keep cleanup failures visible. Test a failing body and broken diagnostic
capture through the actual adapter; check both the report and nonzero process
exit. Test successful invocation and data arguments too.

## Conditions

Start with a built-in condition when it fits. For a domain condition, derive from
`ConditionBaseAttribute`, pass `ConditionMode` to the base constructor, and
implement `GroupName` and `IsConditionMet`. Return the raw predicate:
`IsConditionMet` must not apply `Mode` itself. The framework handles include vs.
exclude. Conditions within one group are ORed; different groups are ANDed.

Make evaluation cheap and deterministic, with a useful `IgnoreMessage`. Default
to non-inherited attributes on classes/methods, matching the built-ins. Avoid
global environment mutations in parallel tests. Verify a false condition really
skips the body and a true one runs it. A skipped case is not evidence that the
tested behavior passed.

## Retry policies

Use `[Retry]` before writing a custom policy. `RetryBaseAttribute.ExecuteAsync`,
`RetryContext`, and `RetryResult` are experimental in the inspected versions and emit
`MSTESTEXP`; acknowledge that locally when the extension actually needs them.
The protected override returns `Task<RetryResult>`. Invoke
`retryContext.ExecuteTaskGetter()` for each retry and call `AddResult` with every
returned array. Return at least one retry attempt; the runner already made the
first attempt before entering this policy.

Inspect **all** results in an attempt. Define retryable outcomes, an attempt
limit, cancellation, delays, and idempotence explicitly. Failed/timed-out results
are different from an inconclusive result. Preserve `AllResults` rather than
fabricating success. MTP can expose superseded attempts; final reports and
VSTest have different reporting behavior. Process-level retries multiply
attribute-level attempts. Keep both disabled when measuring flakiness.

## Programmatic filtering

Register a concrete `ITestFilter` with a public parameterless constructor through
one assembly-level `TestFilterProvider` (the generic form was checked on modern
.NET with 4.4.1 and 4.5.1). Implement `Filter(TestFilterContext)` and deliberately choose
`TestFilterResult.Run`, `.Skip(reason)`, or `.Drop`. Dropping removes the result;
skipping reports it. This runs after command-line selection and before loading
the test class. Compose multiple strategies inside that one provider. The generic
registration is for modern .NET; use the non-generic form on .NET Framework.

The runner can call one provider concurrently, so keep filter state thread-safe.
Keep decisions independent of test-class initialization. A throwing filter
is an error, not a skip. Confirm experimental annotations and analyzer rules in
the installed version; current online SDK guidance can differ from package
metadata. Verify run/skip/drop and malformed registration with a real consumer
when implementing a filter, rather than merely calling `Filter` directly.

Sources: [execution control](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-writing-tests-controlling-execution),
[SDK](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-sdk),
and version-matched `ITestMethod`, `ConditionBaseAttribute`, `RetryResult`, and
`TestFilterProviderAttribute` in [source routes](sources.md).
