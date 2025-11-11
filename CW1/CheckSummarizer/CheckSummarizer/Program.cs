// <copyright file="Program.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace CheckSummarizer;

internal class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine(
            "Hi, this is a utility for calculating md5 hashes across files and folders. usage:\n<path> - single-threaded implementation\n<path> -p - multithreaded implementation");
        var checkSummarizer = new CheckSummarizer();
    }
}
