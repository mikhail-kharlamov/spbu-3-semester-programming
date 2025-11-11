// <copyright file="Program.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace CheckSummarizer;

internal class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length == 0 || args.Length > 2)
        {
            Console.WriteLine("Usage: CheckSummarizer <file> <-p>");
            return;
        }

        var path = args[0];
        var parallel = args.Length > 1;
        var checkSummarizer = new CheckSummarizer();
        var result = string.Empty;
        if (parallel)
        {
            if (args[1] == "-p")
            {
                result = Program.ToHexString(await checkSummarizer.SummarizeSystemObjectParallel(path));
            }
            else
            {
                result = "Usage: CheckSummarizer <file> <-p>";
            }
        }
        else
        {
            result = Program.ToHexString(await checkSummarizer.SummarizeSystemObject(path));
        }

        Console.WriteLine(result);
    }

    private static string ToHexString(byte[] bytes)
    {
        var sb = new System.Text.StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}
