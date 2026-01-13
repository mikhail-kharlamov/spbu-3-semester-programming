// <copyright file="MyNUnit.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using MyNUnit.Attributes;
using MyNUnit.Models;

namespace MyNUnit;

/// <summary>
/// Entry point for running tests in assemblies using the custom MyNUnit framework.
/// </summary>
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
    public static TestResult[] RunAllTests(string path)
    {
        var testClasses = MyNUnit.GetTestClasses(path);
        var results = new ConcurrentBag<TestResult>();
        Parallel.ForEach(
            testClasses,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            testClass =>
            {
                var fileResults = MyNUnit.HandleClass(testClass);
                foreach (var result in fileResults)
                {
                    results.Add(result);
                }
            });
        return results.ToArray();
    }

    private static ConcurrentBag<TestResult> HandleClass(Type type)
    {
        var results = new ConcurrentBag<TestResult>();
        ClassMethodsByAttributes methods;

        try
        {
            methods = MyNUnit.GetAttributeMarkedMethods(type);
        }
        catch (Exception e)
        {
             results.Add(
                 new TestResult(
                 AssemblyName: type.Assembly.GetName().Name ?? string.Empty,
                 ClassName: type.FullName ?? type.Name,
                 MethodName: "ClassStructureDefinition",
                 Status: TestStatus.Errored,
                 Duration: TimeSpan.Zero,
                 Phase: TestPhase.BeforeClass,
                 Message: "Invalid test class structure",
                 Exception: e));
             return results;
        }

        var assemblyName = type.Assembly.GetName().Name ?? type.Assembly.FullName ?? string.Empty;
        var className = type.FullName ?? type.Name;

        if (!MyNUnit.RunBeforeClass(methods, assemblyName, className, results))
        {
            return results;
        }

        MyNUnit.RunTests(methods, type, assemblyName, className, results);
        MyNUnit.RunAfterClass(methods, assemblyName, className, results);

        return results;
    }

    private static bool RunBeforeClass(
        ClassMethodsByAttributes methods,
        string assemblyName,
        string className,
        ConcurrentBag<TestResult> results)
    {
        var success = true;
        foreach (var method in methods.BeforeClassMethods)
        {
            try
            {
                method.Invoke(null, null);
            }
            catch (Exception e)
            {
                success = false;
                var exception = e is TargetInvocationException tie ? tie.InnerException ?? tie : e;

                results.Add(
                    new TestResult(
                    AssemblyName: assemblyName,
                    ClassName: className,
                    MethodName: method.Name,
                    Status: TestStatus.Errored,
                    Duration: TimeSpan.Zero,
                    Phase: TestPhase.BeforeClass,
                    Message: "Exception in BeforeClass",
                    Exception: exception));
            }
        }

        return success;
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
                var myTestAttr = method.GetCustomAttribute<MyTest>();
                var expectedException = myTestAttr?.Expected;
                var ignoreReason = myTestAttr?.Ignore;

                if (ignoreReason != null)
                {
                    sw.Stop();
                    results.Add(
                        new TestResult(
                        AssemblyName: assemblyName,
                        ClassName: className,
                        MethodName: method.Name,
                        Status: TestStatus.Ignored,
                        Duration: sw.Elapsed,
                        Phase: TestPhase.Test,
                        Message: ignoreReason));
                    return;
                }

                try
                {
                    instance ??= Activator.CreateInstance(type);

                    foreach (var before in methods.BeforeMethods)
                    {
                        try
                        {
                            before.Invoke(instance, null);
                        }
                        catch (Exception e)
                        {
                            status = TestStatus.Errored;
                            phase = TestPhase.Before;
                            exception = e is TargetInvocationException tie ? tie.InnerException ?? tie : e;
                            message = $"Exception in Before method {before.Name}";
                            return;
                        }
                    }

                    try
                    {
                        var result = method.Invoke(instance, null);
                        if (result is Task t)
                        {
                            t.GetAwaiter().GetResult();
                        }

                        if (expectedException != null)
                        {
                            status = TestStatus.Failed;
                            message = $"Expected exception {expectedException.Name} but none was thrown.";
                        }
                    }
                    catch (Exception e)
                    {
                        var actualEx = e is TargetInvocationException tie ? tie.InnerException ?? tie : e;

                        if (expectedException != null && expectedException.IsInstanceOfType(actualEx))
                        {
                            status = TestStatus.Passed;
                        }
                        else
                        {
                            status = TestStatus.Failed;
                            phase = TestPhase.Test;
                            exception = actualEx;
                            message = expectedException != null
                                ? $"Expected exception {expectedException.Name} but {actualEx.GetType().Name} was thrown."
                                : "Exception in test method";
                        }
                    }

                    foreach (var after in methods.AfterMethods)
                    {
                        try
                        {
                            after.Invoke(instance, null);
                        }
                        catch (Exception e)
                        {
                            var afterEx = e is TargetInvocationException tie ? tie.InnerException ?? tie : e;

                            if (status == TestStatus.Passed)
                            {
                                status = TestStatus.Errored;
                                phase = TestPhase.After;
                                exception = afterEx;
                                message = $"Exception in After method {after.Name}";
                            }
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
        foreach (var method in methods.AfterClassMethods)
        {
            try
            {
                method.Invoke(null, null);
            }
            catch (Exception e)
            {
                var exception = e is TargetInvocationException tie ? tie.InnerException ?? tie : e;

                results.Add(
                    new TestResult(
                    AssemblyName: assemblyName,
                    ClassName: className,
                    MethodName: method.Name,
                    Status: TestStatus.Errored,
                    Duration: TimeSpan.Zero,
                    Phase: TestPhase.AfterClass,
                    Message: "Exception in AfterClass",
                    Exception: exception));
            }
        }
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
                if (attribute is BeforeClass)
                {
                    ValidateStaticVoidNoArgs(method, nameof(BeforeClass));
                    beforeClass.Add(method);
                }
                else if (attribute is AfterClass)
                {
                    ValidateStaticVoidNoArgs(method, nameof(AfterClass));
                    afterClass.Add(method);
                }
                else if (attribute is Before)
                {
                    ValidateInstanceVoidNoArgs(method, nameof(Before));
                    before.Add(method);
                }
                else if (attribute is After)
                {
                    ValidateInstanceVoidNoArgs(method, nameof(After));
                    after.Add(method);
                }
                else if (attribute is MyTest)
                {
                    ValidateInstanceVoidNoArgs(method, nameof(MyTest));
                    tests.Add(method);
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

    private static void ValidateStaticVoidNoArgs(MethodInfo method, string attributeName)
    {
        if (!method.IsStatic || method.ReturnType != typeof(void) || method.GetParameters().Length > 0)
        {
            throw new InvalidOperationException($"Method {method.Name} marked with {attributeName} must be static, void, and have no parameters.");
        }
    }

    private static void ValidateInstanceVoidNoArgs(MethodInfo method, string attributeName)
    {
        if (method.IsStatic || method.ReturnType != typeof(void) || method.GetParameters().Length > 0)
        {
            throw new InvalidOperationException($"Method {method.Name} marked with {attributeName} must be instance (non-static), void, and have no parameters.");
        }
    }

    private static Type[] GetTestClasses(string mainPath)
    {
        var types = new List<Type>();
        if (Directory.Exists(mainPath))
        {
            var paths = Directory.GetFiles(mainPath, "*.dll", SearchOption.AllDirectories).ToList();
            foreach (var dllPath in paths)
            {
                try
                {
                    types.AddRange(Assembly.LoadFrom(dllPath).ExportedTypes.Where(t => t.IsClass));
                }
                catch (BadImageFormatException)
                {
                }
            }
        }
        else if (File.Exists(mainPath))
        {
            types.AddRange(Assembly.LoadFrom(mainPath).ExportedTypes.Where(t => t.IsClass));
        }
        else
        {
            throw new FileNotFoundException("File or directory not found", mainPath);
        }

        return types.ToArray();
    }
}
