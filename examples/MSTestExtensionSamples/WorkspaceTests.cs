namespace MSTestExtensionSamples;

[TestClass]
public sealed class WorkspaceTests(TestContext context)
{
    private string? _directory;
    private int _initializations;

    [TestInitialize]
    public async Task Initialize()
    {
        Assert.AreEqual(1, ++_initializations, "Each data row must have its own test instance.");
        _directory = Directory.CreateTempSubdirectory("mstest-extensions-").FullName;
        try
        {
            await File.WriteAllTextAsync(Path.Combine(_directory, "value.txt"), "fresh", context.CancellationToken);
        }
        catch
        {
            Directory.Delete(_directory, recursive: true);
            _directory = null;
            throw;
        }
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_directory is not null)
        {
            Directory.Delete(_directory, recursive: true);
            Assert.IsFalse(Directory.Exists(_directory));
        }
    }

    [TestMethod]
    [DataRow("alpha")]
    [DataRow("beta")]
    public async Task Each_row_receives_an_isolated_workspace(string value)
    {
        var file = Path.Combine(_directory!, "value.txt");
        Assert.AreEqual("fresh", await File.ReadAllTextAsync(file, context.CancellationToken));
        await File.WriteAllTextAsync(file, value, context.CancellationToken);
        Assert.AreEqual(value, await File.ReadAllTextAsync(file, context.CancellationToken));
    }
}
