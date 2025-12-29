// <copyright file="Program.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace BenchmarkRunner;

internal class Program
{
    public static void Main(string[] args)
    {
        var sizes = new List<(Size, Size)>
        {
            (new Size(200, 200), new Size(200, 200)),
            (new Size(500, 500), new Size(500, 500)),
            (new Size(1000, 1000), new Size(1000, 1000)),
            (new Size(2000, 2000), new Size(2000, 2000)),
        };

        BenchmarkRunner.RunBenchmarks(sizes, runsPerCase: 5, outputFilePath: "results_blocks.csv");
    }
}
