using System.Runtime.CompilerServices;

namespace MSTestExtensionSamples;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ComposedTestClassAttribute : TestClassAttribute
{
    private static readonly AsyncLocal<bool> Active = new();

    public static bool IsExecuting => Active.Value;

    public override TestMethodAttribute GetTestMethodAttribute(TestMethodAttribute testMethodAttribute)
    {
        ArgumentNullException.ThrowIfNull(testMethodAttribute);
        // Discovery already captured navigation from the original declared attribute.
        return new ExecutionWrapperAttribute(testMethodAttribute);
    }

    private sealed class ExecutionWrapperAttribute(
        TestMethodAttribute inner,
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = -1) : TestMethodAttribute(file, line)
    {
        public override async Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
        {
            var previous = Active.Value;
            Active.Value = true;
            try { return await inner.ExecuteAsync(testMethod); }
            finally { Active.Value = previous; }
        }
    }
}
