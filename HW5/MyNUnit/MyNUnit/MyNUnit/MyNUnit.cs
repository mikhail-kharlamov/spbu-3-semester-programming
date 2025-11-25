using System.Collections.Concurrent;
using System.Reflection;
using MyNUnit.Attributes;
using MyNUnit.Models;

namespace MyNUnit;

public class MyNUnit
{
    public TestResult[] RunAllTests(string path)
    {
        var assemblies = this.GetAssemblies(path);
        var results = new ConcurrentBag<TestResult>();
        Parallel.ForEach(
            assemblies,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            assembly =>
            {
                var fileResults = this.HandleClass(assembly);
                foreach (var result in fileResults)
                {
                    results.Add(result);
                }
            });
        return results.ToArray();
    }

    private ConcurrentBag<TestResult> HandleClass(Type type)
    {
        var methods = this.GetAttributeMarkedMethods(type);
        var results = new ConcurrentBag<TestResult>();
        Parallel.ForEach(
            methods.BeforeClassMethods,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            method =>
            {
                method.Invoke(type, null);
            });
        Parallel.ForEach(
            methods.TestMethods,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            method =>
            {
                foreach (var before in methods.BeforeMethods)
                {
                    before.Invoke(type, null);
                }

                method.Invoke(type, null);

                foreach (var before in methods.AfterMethods)
                {
                    before.Invoke(type, null);
                }
            });
        Parallel.ForEach(
            methods.AfterClassMethods,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            method =>
            {
                method.Invoke(type, null);
            });
        return results;
    }

    private ClassMethodsByAttributes GetAttributeMarkedMethods(Type type)
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

                    case Test:
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

    private Type[] GetAssemblies(string mainPath)
    {
        var assemblies = new List<Type>();
        if (Directory.Exists(mainPath))
        {
            var paths = Directory.GetFiles(mainPath, "*.dll", SearchOption.AllDirectories).ToList();
            paths.AddRange(Directory.GetDirectories(mainPath));
            foreach (var path in paths)
            {
                assemblies.AddRange(this.GetAssemblies(path));
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
