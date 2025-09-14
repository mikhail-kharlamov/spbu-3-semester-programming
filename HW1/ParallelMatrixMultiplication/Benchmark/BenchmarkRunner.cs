// <copyright file="BenchmarkRunner.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Globalization;
using ParallelMatrixMultiplication;

namespace BenchmarkRunner;

/// <summary>
/// Class for benchmarking matrix multiplication.
/// </summary>
public static class BenchmarkRunner
{
    /// <summary>
    /// Method for benchmarking matrix multiplication.
    /// </summary>
    /// <param name="sizes">List with size records for matrices for experience.</param>
    /// <param name="runsPerCase">Count of runs for every multiplication.</param>
    /// <param name="outputFilePath">Path to .csv file for saving experiences results.</param>
    public static void RunBenchmarks(
        List<(Size LeftSize, Size RightSize)> sizes,
        int runsPerCase,
        string outputFilePath)
    {
        using var writer = new StreamWriter(outputFilePath);
        writer.WriteLine("RowsLeft,ColsLeft,RowsRight,ColsRight,Mode,Mean,StdDev");

        foreach (var (leftSize, rightSize) in sizes)
        {
            if (leftSize.Width != rightSize.Height)
            {
                Console.Error.WriteLine("Left size is not correlated with right size.");
                continue;
            }

            Console.WriteLine($"Тест: {leftSize.Width}x{leftSize.Height} * {rightSize.Width}x{rightSize.Height}");

            var left = BenchmarkRunner.GenerateMatrix(leftSize);
            var right = BenchmarkRunner.GenerateMatrix(rightSize);

            var ordinaryTimes =
                BenchmarkRunner.Measure(() => MatrixMultiplication.Multiply(left, right), runsPerCase);
            var parallelTimes =
                BenchmarkRunner.Measure(() => MatrixMultiplication.MultiplyParallel(left, right), runsPerCase);

            BenchmarkRunner.WriteResult(writer, leftSize, rightSize, "Ordinary", ordinaryTimes);
            BenchmarkRunner.WriteResult(writer, leftSize, rightSize, "Parallel", parallelTimes);
        }
    }

    private static List<double> Measure(Action action, int runsPerCase)
    {
        var times = new List<double>(runsPerCase);

        action();

        for (var i = 0; i < runsPerCase; i++)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            times.Add(stopwatch.Elapsed.TotalSeconds);
        }

        return times;
    }

    private static void WriteResult(StreamWriter writer, Size leftSize, Size rightSize, string mode, List<double> times)
    {
        var mean = times.Average();
        var standardDeviation = Math.Sqrt(times.Sum(t => (t - mean) * (t - mean)) / times.Count);

        writer.WriteLine(
            $"{leftSize.Width},{leftSize.Height},{rightSize.Width},{rightSize.Height},{mode}," +
            $"{mean.ToString("F15", CultureInfo.InvariantCulture)}," +
            $"{standardDeviation.ToString("F15", CultureInfo.InvariantCulture)}");
    }

    private static Matrix GenerateMatrix(Size size)
    {
        var random = new Random();
        var matrix = new int[size.Width][];
        for (var i = 0; i < size.Width; i++)
        {
            matrix[i] = new int[size.Height];
            for (var j = 0; j < size.Height; j++)
            {
                matrix[i][j] = random.Next(-100, 100);
            }
        }

        return Matrix.FromArrays(matrix);
    }
}
