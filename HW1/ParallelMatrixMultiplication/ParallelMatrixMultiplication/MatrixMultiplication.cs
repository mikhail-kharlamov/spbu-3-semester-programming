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
            throw new ArgumentException("Количество столбцов левой матрицы не соотносится с количеством строк правой");
        }

        var matrix = new int[left.Rows][];
        for (var i = 0; i < left.Rows; i++)
        {
            matrix[i] = new int[right.Columns];
        }

        for (var i = 0; i < left.Rows; i++)
        {
            for (var j = 0; j < right.Columns; j++)
            {
                matrix[i][j] = MatrixMultiplication.DotProduct(left.GetRow(i), right.GetColumn(j));
            }
        }

        return Matrix.FromArrays(matrix);
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
            throw new ArgumentException("Количество столбцов левой матрицы не соотносится с количеством строк правой");
        }

        var threads = new List<Thread>();
        var matrix = new int[left.Rows][];
        for (var i = 0; i < left.Rows; i++)
        {
            matrix[i] = new int[right.Columns];
        }

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
                                matrix[i][j] = MatrixMultiplication.DotProduct(left.GetRow(i), right.GetColumn(j));
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

        return Matrix.FromArrays(matrix);
    }

    private static int DotProduct(int[] vector1, int[] vector2)
    {
        return vector1.Zip(vector2, (x, y) => x * y).Sum();
    }
}
