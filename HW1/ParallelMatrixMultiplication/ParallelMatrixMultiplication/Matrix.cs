// <copyright file="Matrix.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace ParallelMatrixMultiplication;

/// <summary>
/// Class for integer matrix with  methods for load and upload data to/from file.
/// </summary>
public class Matrix
{
    private readonly int[,] data;

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class from 2D array.
    /// </summary>
    /// <param name="data">2D array with matrix values.</param>
    public Matrix(int[,] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        this.data = (int[,])data.Clone();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class from jagged array.
    /// </summary>
    /// <param name="data">Array with integer arrays.</param>
    /// <exception cref="ArgumentException">Throw exception if arrays don't correspond in size.</exception>
    public Matrix(int[][] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.Length == 0)
        {
            this.data = new int[0, 0];
            return;
        }

        if (data[0] is null)
        {
            throw new ArgumentException("The row of the matrix cannot be null.");
        }

        var columns = data[0].Length;
        if (data.Any(row => row.Length != columns))
        {
            throw new ArgumentException("The rows of the matrix from the array are not related in size.");
        }

        var tmp = new int[data.Length, columns];
        for (var i = 0; i < data.Length; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                tmp[i, j] = data[i][j];
            }
        }

        this.data = tmp;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class from the text file.
    /// </summary>
    /// <param name="filePath">Path to text file for upload.</param>
    public Matrix(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        try
        {
            var lines = File.ReadAllLines(filePath);

            List<int[]> rows = new();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                rows.Add(ParseLine(line));
            }

            if (rows.Count == 0)
            {
                this.data = new int[0, 0];
                return;
            }

            var columns = rows[0].Length;
            var nonConsistent = rows.Where(r => r.Length != columns).ToArray();
            if (nonConsistent.Any())
            {
                throw new ArgumentException("The rows of the matrix from the file are not related in size.");
            }

            var tmp = new int[rows.Count, columns];
            for (var i = 0; i < rows.Count; i++)
            {
                for (var j = 0; j < columns; j++)
                {
                    tmp[i, j] = rows[i][j];
                }
            }

            this.data = tmp;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(
                $"Failed to initialize the matrix from the {filePath} file. Error: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Gets count of rows in matrix.
    /// </summary>
    public int Rows => this.data.GetLength(0);

    /// <summary>
    /// Gets count of columns in matrix.
    /// </summary>
    public int Columns => this.data.GetLength(1);

    /// <summary>
    /// Gets matrix element by indices (read-only indexer).
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <param name="column">Column index.</param>
    /// <returns>Element at [row, column].</returns>
    public int this[int row, int column] => this.data[row, column];

    /// <summary>
    /// Method for loading matrix to text file.
    /// </summary>
    /// <param name="filePath">Path to text file for load.</param>
    public void ToFile(string filePath)
    {
        try
        {
            var lines = new string[this.Rows];
            for (var i = 0; i < this.Rows; i++)
            {
                var row = new string[this.Columns];
                for (var j = 0; j < this.Columns; j++)
                {
                    row[j] = this.data[i, j].ToString();
                }

                lines[i] = string.Join(" ", row);
            }

            File.WriteAllLines(filePath, lines);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"The matrix is not saved in {filePath}. Error saving the matrix: {e.Message}");
            throw;
        }
    }

    /// <summary>
    /// Method for getting matrix row (copy).
    /// </summary>
    /// <param name="row">Row index.</param>
    /// <returns>Integer array with elements from matrix row.</returns>
    public int[] GetRow(int row)
    {
        var result = new int[this.Columns];
        for (var j = 0; j < this.Columns; j++)
        {
            result[j] = this.data[row, j];
        }

        return result;
    }

    /// <summary>
    /// Method for getting matrix column (copy).
    /// </summary>
    /// <param name="column">Column index.</param>
    /// <returns>Integer array with elements from matrix column.</returns>
    public int[] GetColumn(int column)
    {
        var result = new int[this.Rows];
        for (var i = 0; i < this.Rows; i++)
        {
            result[i] = this.data[i, column];
        }

        return result;
    }

    private static int[] ParseLine(string line)
    {
        var splitLine = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new int[splitLine.Length];

        for (var i = 0; i < splitLine.Length; i++)
        {
            try
            {
                result[i] = int.Parse(splitLine[i]);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Inconsistent data in the matrix cells. Error: {e.Message}");
                throw;
            }
        }

        return result;
    }
}
