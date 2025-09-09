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
    /// Tests for creation by fabric method .FromArrays with valid data.
    /// </summary>
    [Test]
    public void FromArrays_ValidData_CreatesMatrix()
    {
        var data = new[]
        {
            new[] { 1, 2, 3 },
            new[] { 4, 5, 6 },
        };

        var matrix = Matrix.FromArrays(data);

        Assert.That(matrix.Rows, Is.EqualTo(2));
        Assert.That(matrix.Columns, Is.EqualTo(3));
        Assert.That(matrix.GetCell(1, 1), Is.EqualTo(5));
    }

    /// <summary>
    /// Tests for creation by fabric method .FromArrays with invalid data.
    /// </summary>
    [Test]
    public void FromArrays_InvalidData_Throws()
    {
        var data = new[]
        {
            new[] { 1, 2 },
            new[] { 3 },
        };

        Assert.Throws<ArgumentException>(() => Matrix.FromArrays(data));
    }

    /// <summary>
    /// Tests for dumping to file by method .ToFile.
    /// </summary>
    [Test]
    public void ToFile_And_FromFile_RoundTrip_Works()
    {
        var data = new[]
        {
            new[] { 1, 2, 3 },
            new[] { 4, 5, 6 },
        };

        var matrix = Matrix.FromArrays(data);

        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".txt");
        matrix.ToFile(tempFile);

        var loaded = Matrix.FromFile(tempFile);
        Assert.That(matrix.Rows, Is.EqualTo(loaded.Rows));
        Assert.That(matrix.Columns, Is.EqualTo(loaded.Columns));
        Assert.That(matrix.GetCell(1, 1), Is.EqualTo(loaded.GetCell(1, 1)));
    }
}
