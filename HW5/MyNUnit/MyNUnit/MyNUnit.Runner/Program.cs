// <copyright file="Program.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>
using MyNUnit.Models;

if (args.Length != 1)
{
    Console.WriteLine("Usage: MyNUnit <path-to-tests-root>");
    return 1;
}

var rootPath = args[0];

if (!Directory.Exists(rootPath) && !File.Exists(rootPath))
{
    Console.WriteLine($"Directory or file not found: {rootPath}");
    return 1;
}

var results = MyNUnit.MyNUnit.RunAllTests(rootPath);

PrintResults(results);

var failed = results.Any(r => r.Status == TestStatus.Failed || r.Status == TestStatus.Errored);
return failed ? 1 : 0;

static void PrintResults(TestResult[] results)
{
    foreach (var result in results.OrderBy(r => r.AssemblyName)
                             .ThenBy(r => r.ClassName)
                             .ThenBy(r => r.MethodName))
    {
        var fullName = $"{result.AssemblyName}.{result.ClassName}.{result.MethodName}";
        var milliseconds = (int)result.Duration.TotalMilliseconds;

        switch (result.Status)
        {
            case TestStatus.Passed:
                Console.WriteLine($"[PASS]    {fullName} ({milliseconds} ms)");
                break;
            case TestStatus.Failed:
                Console.WriteLine($"[FAIL]    {fullName} ({milliseconds} ms)");
                PrintDetails(result);
                break;
            case TestStatus.Errored:
                Console.WriteLine($"[ERROR]   {fullName} ({milliseconds} ms)");
                PrintDetails(result);
                break;
            case TestStatus.Ignored:
                Console.WriteLine($"[IGNORED] {fullName} ({milliseconds} ms)");
                if (!string.IsNullOrEmpty(result.Message))
                {
                    Console.WriteLine($"          Reason: {result.Message}");
                }

                Console.WriteLine();
                break;
        }
    }

    var total = results.Length;
    var passed = results.Count(r => r.Status == TestStatus.Passed);
    var failed = results.Count(r => r.Status == TestStatus.Failed);
    var errored = results.Count(r => r.Status == TestStatus.Errored);
    var ignored = results.Count(r => r.Status == TestStatus.Ignored);

    Console.WriteLine("===== Summary =====");
    Console.WriteLine($"Total tests: {total}");
    Console.WriteLine($" Passed: {passed}");
    Console.WriteLine($" Failed: {failed}");
    Console.WriteLine($" Errored: {errored}");
    Console.WriteLine($" Ignored: {ignored}");
}

static void PrintDetails(TestResult result)
{
    Console.WriteLine($"         Phase: {result.Phase}");
    if (!string.IsNullOrEmpty(result.Message))
    {
        Console.WriteLine($"         Message: {result.Message}");
    }

    if (result.Exception != null)
    {
        Console.WriteLine($"         Exception: {result.Exception.GetType().FullName}: {result.Exception.Message}");
    }

    Console.WriteLine();
}
