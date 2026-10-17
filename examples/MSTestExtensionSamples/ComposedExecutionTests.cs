namespace MSTestExtensionSamples;

[ComposedTestClass]
public sealed class ComposedExecutionTests(TestContext context)
{
    [TestMethod]
    [DataRow("plain", 5)]
    [DataRow("é", 2)]
    public void Class_wrapper_preserves_data_arguments(string text, int expectedBytes)
    {
        Assert.IsTrue(ComposedTestClassAttribute.IsExecuting);
#if MSTEST_NATIVE_CONTEXT
        Assert.AreEqual(context.CancellationToken, ComposedTestClassAttribute.CurrentCancellationToken);
#else
        Assert.IsNotNull(context.TestName);
#endif
        Assert.AreEqual(expectedBytes, System.Text.Encoding.UTF8.GetByteCount(text));
    }
}
