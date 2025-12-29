// <copyright file="MatrixMultiplication.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace ParallelMatrixMultiplication;

/// <summary>
/// Class with static methods for matrix multiplication.
/// </summary>
public static class MatrixMultiplication
{
    /// <summary>
    /// Method for matrix multiplication.
    /// </summary>
    /// <param name="left">Left matrix.</param>
    /// <param name="right">Right matrix.</param>
    /// <returns>Matrix object - multiplication of left and right.</returns>
    /// <exception cref="ArgumentException">If left columns count doesn't correspond with right rows count.</exception>
    public static Matrix Multiply(Matrix left, Matrix right)
    {
        if (left.Columns != right.Rows)
        {
            throw new ArgumentException(
                "The number of columns of the left matrix does not correspond to the number of rows of the right one.");
        }

        var result = new int[left.Rows, right.Columns];

        for (var i = 0; i < left.Rows; i++)
        {
            for (var j = 0; j < right.Columns; j++)
            {
                result[i, j] = DotProduct(left, i, right, j);
            }
        }

        return new Matrix(result);
    }

    /// <summary>
    /// Method for parallel matrix multiplication.
    /// </summary>
    /// <param name="left">Left matrix.</param>
    /// <param name="right">Right matrix.</param>
    /// <returns>Matrix object - multiplication of left and right.</returns>
    /// <exception cref="ArgumentException">If left columns count doesn't correspond with right rows count.</exception>
    public static Matrix MultiplyParallel(Matrix left, Matrix right)
    {
        if (left.Columns != right.Rows)
        {
            throw new ArgumentException(
                "The number of columns of the left matrix does not correspond to the number of rows of the right one");
        }

        var threads = new List<Thread>();
        var result = new int[left.Rows, right.Columns];

        var processorCount = Environment.ProcessorCount;
        var blockSize = Math.Max(1, left.Rows / processorCount);

        for (var startRow = 0; startRow < left.Rows; startRow += blockSize)
        {
            var blockStart = startRow;
            var blockEnd = Math.Min(startRow + blockSize, left.Rows);

            var thread = new Thread(
                () =>
                {
                    for (var i = blockStart; i < blockEnd; i++)
                    {
                        for (var j = 0; j < right.Columns; j++)
                        {
                            result[i, j] = DotProduct(left, i, right, j);
                        }
                    }
                });

            threads.Add(thread);
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return new Matrix(result);
    }

    private static int DotProduct(Matrix left, int leftRow, Matrix right, int rightColumn)
    {
        var sum = 0;
        for (var k = 0; k < left.Columns; k++)
        {
            sum += left[leftRow, k] * right[k, rightColumn];
        }

        return sum;
    }
}
