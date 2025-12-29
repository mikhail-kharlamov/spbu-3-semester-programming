// <copyright file="MatrixTests.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace ParallelMatrixMultiplication;

/// <summary>
/// Class with tests for class Matrix.
/// </summary>
public class MatrixTests
{
    /// <summary>
    /// Tests for creation by constructor from jagged arrays with valid data.
    /// </summary>
    [Test]
    public void Constructor_JaggedArrays_ValidData_CreatesMatrix()
    {
        var data = new[]
        {
            new[] { 1, 2, 3 },
            new[] { 4, 5, 6 },
        };

        var matrix = new Matrix(data);

        Assert.That(matrix.Rows, Is.EqualTo(2));
        Assert.That(matrix.Columns, Is.EqualTo(3));
        Assert.That(matrix[1, 1], Is.EqualTo(5));
    }

    /// <summary>
    /// Tests for creation by constructor from jagged arrays with invalid data.
    /// </summary>
    [Test]
    public void Constructor_JaggedArrays_InvalidData_Throws()
    {
        var data = new[]
        {
            new[] { 1, 2 },
            new[] { 3 },
        };

        Assert.Throws<ArgumentException>(() => _ = new Matrix(data));
    }

    /// <summary>
    /// Tests for dumping to file by method .ToFile and loading via constructor.
    /// </summary>
    [Test]
    public void ToFile_And_Constructor_RoundTrip_Works()
    {
        var data = new[]
        {
            new[] { 1, 2, 3 },
            new[] { 4, 5, 6 },
        };

        var matrix = new Matrix(data);

        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".txt");
        try
        {
            matrix.ToFile(tempFile);

            var loaded = new Matrix(tempFile);
            Assert.That(matrix.Rows, Is.EqualTo(loaded.Rows));
            Assert.That(matrix.Columns, Is.EqualTo(loaded.Columns));
            Assert.That(matrix[1, 1], Is.EqualTo(loaded[1, 1]));
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    /// Test for inconsistent rows lengths in file.
    /// </summary>
    [Test]
    public void Constructor_File_InconsistentRows_Throws()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".txt");
        try
        {
            File.WriteAllLines(
                tempFile,
                ["1 2 3", "4 5"]);
            Assert.Throws<ArgumentException>(() => _ = new Matrix(tempFile));
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    /// Test for throwing exception if data is not integer number.
    /// </summary>
    [Test]
    public void Constructor_File_InvalidNumbers_Throws()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".txt");
        try
        {
            File.WriteAllText(tempFile, "1 2 a");
            Assert.Throws<FormatException>(() => _ = new Matrix(tempFile));
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    /// Test for GetRow and GetColumn methods.
    /// </summary>
    [Test]
    public void GetRow_And_GetColumn_ReturnsExpected()
    {
        var data = new[]
        {
            new[] { 1, 2, 3 },
            new[] { 4, 5, 6 },
        };

        var matrix = new Matrix(data);

        Assert.That(matrix.GetRow(0), Is.EquivalentTo(new[] { 1, 2, 3 }));
        Assert.That(matrix.GetColumn(2), Is.EquivalentTo(new[] { 3, 6 }));
    }
}
