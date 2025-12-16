namespace Reflector.Tests;

/// <summary>
/// Provides NUnit test cases for the <see cref="Reflector"/> class.
/// This test fixture demonstrates two main testing techniques:
/// 1. Capturing and asserting console output.
/// 2. Verifying that a type structure is written to a file in the temporary directory.
/// </summary>
public class Tests
{
    private TextWriter? originalOut;

    private StringWriter? outBuffer;

    /// <summary>
    /// Redirects <see cref="Console.Out"/> to an in-memory <see cref="StringWriter"/>
    /// before each test.
    /// This allows tests to assert on text that production code writes to the console
    /// without polluting the real test runner output.
    /// </summary>
    [SetUp]
    public void CaptureConsole()
    {
        this.originalOut = Console.Out;
        this.outBuffer = new StringWriter();
        Console.SetOut(this.outBuffer);
    }

    /// <summary>
    /// Restores <see cref="Console.Out"/> to its original value and disposes
    /// the in-memory buffer after each test.
    /// This ensures that changes to the global console output stream are reverted,
    /// and that the temporary writer used for assertions is properly cleaned up.
    /// </summary>
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

    /// <summary>
    /// Verifies that <see cref="Reflector.DiffClasses(Type, Type)"/> writes
    /// the expected difference report between two test classes to the console.
    /// The method relies on console redirection performed in <see cref="CaptureConsole"/>
    /// and asserts that the captured output exactly matches the expected multiline string.
    /// </summary>
    [Test]
    public void DiffClassesSimpleTest()
    {
        var reflector = new Reflector();
        reflector.DiffClasses(typeof(TestClass1), typeof(TestClass2));
        var output = this.outBuffer!.ToString();
        Assert.That(output, Is.EqualTo("TestClass1 fields that not in TestClass2 fields:\n    testField1;\n    StaticTestField1;\nTestClass2 fields that not in TestClass1 fields:\n    testField11;\n    StaticTestField2;\n"));
    }

    /// <summary>
    /// Verifies that <see cref="Reflector.PrintStructure(Type)"/> writes
    /// the structure of a given type into a file located in the system
    /// temporary directory.
    /// The test temporarily changes the current working directory to the
    /// temp folder so that the method, which uses a relative path, writes
    /// into a known location. It then asserts that the file is created
    /// and can be read, and finally removes the file to keep the environment clean.
    /// </summary>
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
            Assert.That(output, Is.EqualTo("public class TestClass1\n{\n    private Int32 testField1 = null!;\n    public String TestField2 = null!;\n    public static Int32 StaticTestField1 = 3;\n}\n"));
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
