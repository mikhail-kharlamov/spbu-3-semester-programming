using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using MyNUnit.Attributes;
using MyNUnit.Models;

namespace MyNUnit;

/// <summary>
/// Entry point for running tests in assemblies using the custom MyNUnit framework.
/// </summary>
/// <remarks>
/// This class discovers all test assemblies under the specified path and executes
/// all tests found in them in parallel, returning a flat list of test results.
/// </remarks>
public static class MyNUnit
{
    /// <summary>
    /// Discovers and runs all tests found in assemblies under the specified path.
    /// </summary>
    /// <param name="path">
    /// Root directory that will be scanned recursively for test assemblies.
    /// </param>
    /// <returns>
    /// An array of <see cref="TestResult"/> objects describing the outcome of each executed test.
    /// </returns>
    /// <remarks>
    /// Assemblies are processed in parallel, while tests inside a single assembly or test class
    /// may be executed according to the test engine's internal scheduling strategy.
    /// </remarks>
    public static TestResult[] RunAllTests(string path)
    {
        var assemblies = MyNUnit.GetAssemblies(path);
        var results = new ConcurrentBag<TestResult>();
        Parallel.ForEach(
            assemblies,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            assembly =>
            {
                var fileResults = MyNUnit.HandleClass(assembly);
                foreach (var result in fileResults)
                {
                    results.Add(result);
                }
            });
        return results.ToArray();
    }

    private static ConcurrentBag<TestResult> HandleClass(Type type)
    {
        var methods = MyNUnit.GetAttributeMarkedMethods(type);
        var results = new ConcurrentBag<TestResult>();

        var assemblyName = type.Assembly.GetName().Name ?? type.Assembly.FullName ?? string.Empty;
        var className = type.FullName ?? type.Name;

        MyNUnit.RunBeforeClass(methods, assemblyName, className, results);
        MyNUnit.RunTests(methods, type, assemblyName, className, results);
        MyNUnit.RunAfterClass(methods, assemblyName, className, results);

        return results;
    }

    private static void RunBeforeClass(
        ClassMethodsByAttributes methods,
        string assemblyName,
        string className,
        ConcurrentBag<TestResult> results)
    {
        Parallel.ForEach(
            methods.BeforeClassMethods,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            method =>
            {
                var sw = Stopwatch.StartNew();

                var status = TestStatus.Passed;
                Exception? exception = null;

                try
                {
                    method.Invoke(null, null);
                }
                catch (TargetInvocationException e)
                {
                    status = TestStatus.Failed;
                    exception = e.InnerException ?? e;
                }
                catch (Exception e)
                {
                    status = TestStatus.Failed;
                    exception = e;
                }
                finally
                {
                    sw.Stop();
                }

                results.Add(new TestResult(
                    AssemblyName: assemblyName,
                    ClassName: className,
                    MethodName: method.Name,
                    Status: status,
                    Duration: sw.Elapsed,
                    Phase: TestPhase.BeforeClass,
                    Message: status == TestStatus.Failed ? "Exception in BeforeClass" : null,
                    Exception: exception));
            });
    }

    private static void RunTests(
        ClassMethodsByAttributes methods,
        Type type,
        string assemblyName,
        string className,
        ConcurrentBag<TestResult> results)
    {
        Parallel.ForEach(
            methods.TestMethods,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            method =>
            {
                var sw = Stopwatch.StartNew();

                var status = TestStatus.Passed;
                TestPhase? phase = TestPhase.Test;
                Exception? exception = null;
                string? message = null;

                object? instance = null;

                try
                {
                    instance ??= Activator.CreateInstance(type);

                    // Before
                    foreach (var before in methods.BeforeMethods)
                    {
                        try
                        {
                            before.Invoke(instance, null);
                        }
                        catch (TargetInvocationException e)
                        {
                            status = TestStatus.Failed;
                            phase = TestPhase.Before;
                            exception = e.InnerException ?? e;
                            message = $"Exception in Before method {before.Name}";
                            return;
                        }
                        catch (Exception e)
                        {
                            status = TestStatus.Failed;
                            phase = TestPhase.Before;
                            exception = e;
                            message = $"Exception in Before method {before.Name}";
                            return;
                        }
                    }

                    // Test
                    try
                    {
                        var result = method.Invoke(instance, null);

                        if (result is Task t)
                        {
                            t.GetAwaiter().GetResult();
                        }
                    }
                    catch (TargetInvocationException e)
                    {
                        status = TestStatus.Failed;
                        phase = TestPhase.Test;
                        exception = e.InnerException ?? e;
                        message = "Exception in test method";
                        return;
                    }
                    catch (Exception e)
                    {
                        status = TestStatus.Failed;
                        phase = TestPhase.Test;
                        exception = e;
                        message = "Exception in test method";
                        return;
                    }

                    // After
                    foreach (var after in methods.AfterMethods)
                    {
                        try
                        {
                            after.Invoke(instance, null);
                        }
                        catch (TargetInvocationException e)
                        {
                            status = TestStatus.Failed;
                            phase = TestPhase.After;
                            exception = e.InnerException ?? e;
                            message = $"Exception in After method {after.Name}";
                            return;
                        }
                        catch (Exception e)
                        {
                            status = TestStatus.Failed;
                            phase = TestPhase.After;
                            exception = e;
                            message = $"Exception in After method {after.Name}";
                            return;
                        }
                    }
                }
                finally
                {
                    sw.Stop();

                    results.Add(
                        new TestResult(
                        AssemblyName: assemblyName,
                        ClassName: className,
                        MethodName: method.Name,
                        Status: status,
                        Duration: sw.Elapsed,
                        Phase: phase,
                        Message: message,
                        Exception: exception));
                }
            });
    }

    private static void RunAfterClass(
        ClassMethodsByAttributes methods,
        string assemblyName,
        string className,
        ConcurrentBag<TestResult> results)
    {
        Parallel.ForEach(
            methods.AfterClassMethods,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            method =>
            {
                var sw = Stopwatch.StartNew();

                var status = TestStatus.Passed;
                Exception? exception = null;

                try
                {
                    method.Invoke(null, null);
                }
                catch (TargetInvocationException e)
                {
                    status = TestStatus.Failed;
                    exception = e.InnerException ?? e;
                }
                catch (Exception e)
                {
                    status = TestStatus.Failed;
                    exception = e;
                }
                finally
                {
                    sw.Stop();
                }

                results.Add(
                    new TestResult(
                    AssemblyName: assemblyName,
                    ClassName: className,
                    MethodName: method.Name,
                    Status: status,
                    Duration: sw.Elapsed,
                    Phase: TestPhase.AfterClass,
                    Message: status == TestStatus.Failed ? "Exception in AfterClass" : null,
                    Exception: exception));
            });
    }


    private static ClassMethodsByAttributes GetAttributeMarkedMethods(Type type)
    {
        List<MethodInfo> beforeClass = new();
        List<MethodInfo> afterClass = new();
        List<MethodInfo> tests = new();
        List<MethodInfo> before = new();
        List<MethodInfo> after = new();
        foreach (var method in type.GetMethods())
        {
            foreach (var attribute in method.GetCustomAttributes())
            {
                switch (attribute)
                {
                    case BeforeClass:
                        beforeClass.Add(method);
                        break;

                    case AfterClass:
                        afterClass.Add(method);
                        break;

                    case Before:
                        before.Add(method);
                        break;

                    case After:
                        after.Add(method);
                        break;

                    case MyTest:
                        tests.Add(method);
                        break;
                }
            }
        }

        return new ClassMethodsByAttributes(
            tests.ToArray(),
            beforeClass.ToArray(),
            afterClass.ToArray(),
            before.ToArray(),
            after.ToArray());
    }

    private static Type[] GetAssemblies(string mainPath)
    {
        var assemblies = new List<Type>();
        if (Directory.Exists(mainPath))
        {
            var paths = Directory.GetFiles(mainPath, "*.dll", SearchOption.AllDirectories).ToList();
            paths.AddRange(Directory.GetDirectories(mainPath));
            foreach (var path in paths)
            {
                assemblies.AddRange(MyNUnit.GetAssemblies(path));
            }
        }

        if (File.Exists(mainPath))
        {
            assemblies.AddRange(Assembly.LoadFrom(mainPath).ExportedTypes.Where(t => t.IsClass));
        }

        if (assemblies.Count == 0)
        {
            throw new FileNotFoundException("File not found", mainPath);
        }

        return assemblies.ToArray();
    }
}
