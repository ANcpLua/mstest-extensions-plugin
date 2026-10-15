namespace MSTestExtensionSamples;

[ComposedTestClass]
public sealed class ComposedExecutionTests
{
    [TestMethod]
    [DataRow("plain", 5)]
    [DataRow("é", 2)]
    public void Class_wrapper_preserves_data_arguments(string text, int expectedBytes)
    {
        Assert.IsTrue(ComposedTestClassAttribute.IsExecuting);
        Assert.AreEqual(expectedBytes, System.Text.Encoding.UTF8.GetByteCount(text));
    }
}
