using MyNUnit.Models;

if (args.Length != 1)
{
    Console.WriteLine("Usage: MiniTestRunner <path-to-tests-root>");
    return 1;
}

var rootPath = args[0];

if (!Directory.Exists(rootPath))
{
    Console.WriteLine($"Directory not found: {rootPath}");
    return 1;
}

var engine = new MyNUnit.MyNUnit();
var results = engine.RunAllTests(rootPath);

PrintResults(results);

var failed = results.Any(r => r.Status == TestStatus.Failed);
return failed ? 1 : 0;

static void PrintResults(TestResult[] results)
{
    foreach (var r in results.OrderBy(r => r.AssemblyName)
                             .ThenBy(r => r.ClassName)
                             .ThenBy(r => r.MethodName))
    {
        var fullName = $"{r.AssemblyName}.{r.ClassName}.{r.MethodName}";
        var ms = (int)r.Duration.TotalMilliseconds;

        switch (r.Status)
        {
            case TestStatus.Passed:
                Console.WriteLine($"[PASS]    {fullName} ({ms} ms)");
                break;
            case TestStatus.Failed:
                Console.WriteLine($"[FAIL]    {fullName} ({ms} ms)");
                Console.WriteLine($"         Phase: {r.Phase}");
                if (!string.IsNullOrEmpty(r.Message))
                {
                    Console.WriteLine($"         Message: {r.Message}");
                }

                if (r.Exception != null)
                {
                    Console.WriteLine($"         Exception: {r.Exception.GetType().FullName}: {r.Exception.Message}");
                }

                Console.WriteLine();
                break;
            case TestStatus.Ignored:
                Console.WriteLine($"[IGNORED] {fullName} ({ms} ms)");
                if (!string.IsNullOrEmpty(r.Message))
                {
                    Console.WriteLine($"          Reason: {r.Message}");
                }

                Console.WriteLine();
                break;
        }
    }

    var total = results.Length;
    var passed = results.Count(r => r.Status == TestStatus.Passed);
    var failed = results.Count(r => r.Status == TestStatus.Failed);
    var ignored = results.Count(r => r.Status == TestStatus.Ignored);

    Console.WriteLine("===== Summary =====");
    Console.WriteLine($"Total tests: {total}");
    Console.WriteLine($" Passed: {passed}");
    Console.WriteLine($" Failed: {failed}");
    Console.WriteLine($" Ignored: {ignored}");
}
