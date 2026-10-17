using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MSTestExtensionSamples;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ComposedTestClassAttribute : TestClassAttribute
{
    private const string MissingInvocationContext = "Class composition requires a native MSTest invocation context.";
    private static readonly AsyncLocal<Invocation?> Active = new();

    public static bool IsExecuting => Active.Value is not null;
#if MSTEST_NATIVE_CONTEXT
    public static CancellationToken CurrentCancellationToken => Active.Value?.CancellationToken
        ?? throw new InvalidOperationException(MissingInvocationContext);
#endif

    public override TestMethodAttribute GetTestMethodAttribute(TestMethodAttribute testMethodAttribute)
    {
        ArgumentNullException.ThrowIfNull(testMethodAttribute);
        // Discovery already captured navigation from the original declared attribute.
        return new ExecutionWrapperAttribute(testMethodAttribute);
    }

    private sealed record Invocation(CancellationToken CancellationToken);

    private sealed class ExecutionWrapperAttribute(
        TestMethodAttribute inner,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = -1) : TestMethodAttribute(file, line)
    {
#if MSTEST_NATIVE_CONTEXT
        [Experimental("MSTESTEXP")]
#endif
        public override async Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
        {
#if MSTEST_NATIVE_CONTEXT
            var context = TestContext.Current ?? throw new InvalidOperationException(MissingInvocationContext);
            var token = context.CancellationToken;
            token.ThrowIfCancellationRequested();
#else
            // This version has no native context accessor; this example owns no cancellable work.
            var token = CancellationToken.None;
#endif
            var previous = Active.Value;
            Active.Value = new Invocation(token);
            try { return await inner.ExecuteAsync(testMethod); }
            finally { Active.Value = previous; }
        }
    }
}
