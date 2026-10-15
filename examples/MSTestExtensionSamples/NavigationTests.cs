using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace MSTestExtensionSamples;

[TestClass]
public sealed class NavigationTests(TestContext context)
{
    [TestMethod]
    [Timeout(30_000, CooperativeCancellation = true)]
    public async Task Discovery_reports_original_locations_for_declared_and_wrapped_attributes()
    {
        using var process = Process.Start(new ProcessStartInfo("dotnet")
        {
            ArgumentList = { typeof(NavigationTests).Assembly.Location, "--list-tests", "json" },
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        }) ?? throw new InvalidOperationException("Could not start MSTest discovery.");

        try
        {
            var output = process.StandardOutput.ReadToEndAsync(context.CancellationToken);
            var error = process.StandardError.ReadToEndAsync(context.CancellationToken);
            await process.WaitForExitAsync(context.CancellationToken);
            Assert.AreEqual(0, process.ExitCode, await error);
            using var document = JsonDocument.Parse(await output);
            var tests = document.RootElement.GetProperty("tests").EnumerateArray().ToArray();

            Type[] classes = [typeof(ComposedExecutionTests), typeof(ExecutionExtensionTests)];
            foreach (var testClass in classes)
            {
                foreach (var method in testClass.GetMethods().Where(method => method.IsDefined(typeof(TestMethodAttribute))))
                {
                    var original = method.GetCustomAttribute<TestMethodAttribute>()!;
                    Assert.Contains(testClass.Name + ".cs", original.DeclaringFilePath);
                    Assert.IsNotNull(original.DeclaringLineNumber);
                    Assert.IsTrue(original.DeclaringLineNumber > 0);

                    var rows = tests.Where(test =>
                        test.GetProperty("type").GetProperty("typeName").GetString() == testClass.Name &&
                        test.GetProperty("type").GetProperty("methodName").GetString() == method.Name).ToArray();
                    Assert.IsNotEmpty(rows);
                    foreach (var row in rows)
                    {
                        var location = row.GetProperty("location");
                        Assert.AreEqual(original.DeclaringFilePath, location.GetProperty("file").GetString());
                        Assert.AreEqual(original.DeclaringLineNumber, location.GetProperty("lineStart").GetInt32());
                        Assert.AreEqual(original.DeclaringLineNumber, location.GetProperty("lineEnd").GetInt32());
                    }
                }
            }
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync(CancellationToken.None);
            }
        }
    }
}
