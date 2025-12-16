namespace Reflector.Tests;

/// <summary>
///...
/// </summary>
public class Tests
{
    private TextWriter? originalOut;
    private StringWriter? outBuffer;

    [SetUp]
    public void CaptureConsole()
    {
        this.originalOut = Console.Out;
        this.outBuffer = new StringWriter();
        Console.SetOut(this.outBuffer);
    }

    [TearDown]
    public void RestoreConsole()
    {
        if (this.originalOut is not null)
        {
            Console.SetOut(this.originalOut);
        }

        if (this.outBuffer is not null)
        {
            this.outBuffer.Dispose();
        }
    }

    [Test]
    public void Test1()
    {
        var reflector = new Reflector();
        reflector.DiffClasses(typeof(TestClass1), typeof(TestClass2));
        var output = this.outBuffer!.ToString();
        Assert.That(output, Is.EqualTo("TestClass1 fields that not in TestClass2 fields:\n    testField1;\n    StaticTestField1;\nTestClass2 fields that not in TestClass1 fields:\n    testField11;\n    StaticTestField2;\n"));
    }

    [Test]
    public void PrintStructureWritesExpectedContentToTempFile()
    {
        var type = typeof(TestClass1);

        var tempDir = Path.GetTempPath();
        Directory.SetCurrentDirectory(tempDir);
        var path = Path.Combine(tempDir, $"{type.Name}.cs");

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        var reflector = new Reflector();

        try
        {
            reflector.PrintStructure(type);

            Assert.That(File.Exists(path), Is.True);

            var output = File.ReadAllText(path);
            Assert.That(output, Is.EqualTo(File.ReadAllText(path)));
        }
        finally
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
