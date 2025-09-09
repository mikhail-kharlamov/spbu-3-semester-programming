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

        var threads = new Thread[left.Rows];
        var matrix = new int[left.Rows][];
        for (var i = 0; i < left.Rows; i++)
        {
            matrix[i] = new int[right.Columns];
        }

        for (var i = 0; i < left.Rows; i++)
        {
            var row = i;
            threads[i] = new Thread(
                () =>
                {
                    for (var j = 0; j < right.Columns; j++)
                    {
                        matrix[row][j] = MatrixMultiplication.DotProduct(left.GetRow(row), right.GetColumn(j));
                    }
                });
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
