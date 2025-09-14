// <copyright file="MatrixMultiplicationTests.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace ParallelMatrixMultiplication;

/// <summary>
/// Class with tests for matrix multiplication.
/// </summary>
public class MatrixMultiplicationTests
{
    /// <summary>
    /// Test that checks that Multiply method works.
    /// </summary>
    [Test]
    public void Multiply_Simple2x2_Works()
    {
        var left = Matrix.FromArrays(
        [
           [1, 2],
           [3, 4],
        ]);
        var right = Matrix.FromArrays(
        [
            [2, 0],
            [1, 2]
        ]);

        var result = MatrixMultiplication.Multiply(left, right);
        Assert.That(result.GetRow(0), Is.EquivalentTo(new[] { 4, 4 }));
        Assert.That(result.GetRow(1), Is.EquivalentTo(new[] { 10, 8 }));
    }

    /// <summary>
    /// Test that checks that MultiplyParallel method works.
    /// </summary>
    [Test]
    public void MultiplyParallel_Simple2x2_Works()
    {
        var left = Matrix.FromArrays(
            [
                [1, 2],
                [3, 4],
            ]);
        var right = Matrix.FromArrays(
            [
                [2, 0],
                [1, 2]
            ]);

        var result = MatrixMultiplication.MultiplyParallel(left, right);
        Assert.That(result.GetRow(0), Is.EquivalentTo(new[] { 4, 4 }));
        Assert.That(result.GetRow(1), Is.EquivalentTo(new[] { 10, 8 }));
    }

    /// <summary>
    /// Test that checks similarity of sequential and parallel computation of matrix multiplication.
    /// </summary>
    [Test]
    public void MultiplyParallel_GivesSameResultAsSequential()
    {
        var left = Matrix.FromArrays(
        [
            [1, 2, 3],
            [4, 5, 6]
        ]);
        var right = Matrix.FromArrays(
            [
                [7, 8],
                [9, 10],
                [11, 12]
            ]);

        var sequential = MatrixMultiplication.Multiply(left, right);
        var parallel = MatrixMultiplication.MultiplyParallel(left, right);

        for (var i = 0; i < sequential.Rows; i++)
        {
            for (var j = 0; j < sequential.Columns; j++)
            {
                Assert.That(parallel.GetCell(i, j), Is.EqualTo(sequential.GetCell(i, j)));
            }
        }
    }
}
