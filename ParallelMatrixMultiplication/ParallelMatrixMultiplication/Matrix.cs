// <copyright file="Matrix.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace ParallelMatrixMultiplication;

/// <summary>
/// Class for integer matrix with  methods for load and upload data to/from file.
/// </summary>
public class Matrix
{
    private int[][] data = Array.Empty<int[]>();

    /// <summary>
    /// Gets count of rows in matrix.
    /// </summary>
    public int Rows { get; private set; }

    /// <summary>
    /// Gets count of columns in matrix.
    /// </summary>
    public int Columns { get; private set; }

    /// <summary>
    /// Fabric method for uploading matrix from the array with integer arrays.
    /// </summary>
    /// <param name="data">Array with integer arrays.</param>
    /// <returns>Matrix object with data from arrays.</returns>
    /// <exception cref="Exception">Throw exception if arrays don't correspond in size.</exception>
    public static Matrix FromArrays(int[][] data)
    {
        var nonConsistent = data.Where(i => i.Length != data[0].Length).ToArray();
        if (nonConsistent.Any())
        {
            throw new ArgumentException("Строки матрицы из массива не соотносятся по размерам.");
        }

        var matrix = new Matrix();
        matrix.Columns = data[0].Length;
        matrix.Rows = data.Length;
        matrix.data = data;
        return matrix;
    }

    /// <summary>
    /// Fabric method for uploading matrix from the text file.
    /// </summary>
    /// <param name="filePath">path to text file for upload.</param>
    /// <returns>Matrix object with data from file.</returns>
    public static Matrix FromFile(string filePath)
    {
        try
        {
            var lines = File.ReadAllLines(filePath);
            List<int[]> matrix = new();
            foreach (var line in lines)
            {
                var vector = Matrix.ParseLine(line);
                matrix.Add(vector);
            }

            var resultMatrix = Matrix.FromArrays(matrix.ToArray());
            return resultMatrix;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(
                $"Не удалось проинициализировать матрицу из файла {filePath}. Ошибка: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Method for loading matrix to text file.
    /// </summary>
    /// <param name="filePath">path to text file for load.</param>
    public void ToFile(string filePath)
    {
        try
        {
            var lines =
                from row in this.data
                select string.Join(" ", row);
            File.WriteAllLines(filePath, lines);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Матрица не сохранена в {filePath}. Ошибка при сохранении матрицы: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Method for getting matrix cell.
    /// </summary>
    /// <param name="row">row index of element.</param>
    /// <param name="column">column index of element.</param>
    /// <returns>integer element from matrix cell.</returns>
    public int GetCell(int row, int column)
    {
        return this.data[row][column];
    }

    /// <summary>
    /// Method for getting matrix row.
    /// </summary>
    /// <param name="row">row index.</param>
    /// <returns>integer array with elements from matrix row.</returns>
    public int[] GetRow(int row)
    {
        return this.data[row];
    }

    /// <summary>
    /// Method for getting matrix column.
    /// </summary>
    /// <param name="column">column index.</param>
    /// <returns>integer array with element from from matrix column.</returns>
    public int[] GetColumn(int column)
    {
        var vector =
            from row in this.data
            select row[column];
        return vector.ToArray();
    }

    private static int[] ParseLine(string line)
    {
        var splitLine = line.Split(" ");
        var result = new int[splitLine.Length];
        for (var i = 0; i < splitLine.Length; i++)
        {
            try
            {
                result[i] = int.Parse(splitLine[i]);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Неконсистентые данные в ячейках матрицы. Ошибка: {e.Message}");
                throw;
            }
        }

        return result;
    }
}
