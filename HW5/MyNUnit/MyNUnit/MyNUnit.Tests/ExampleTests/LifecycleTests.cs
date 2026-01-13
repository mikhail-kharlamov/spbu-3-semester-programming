// <copyright file="LifecycleTests.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>
using MyNUnit.Attributes;

namespace MyNUnit.Tests.ExampleTests;

/// <summary>
/// Sample test class used to verify correct invocation order and call counts
/// for <c>BeforeClass</c>, <c>AfterClass</c>, <c>Before</c>, and <c>After</c> methods
/// in the MyNUnit framework.
/// </summary>
public class LifecycleTests
{
    /// <summary>
    /// Number of times the <see cref="BeforeClassMethod"/> has been called.
    /// </summary>
    public static int BeforeClassCalls;

    /// <summary>
    /// Number of times the <see cref="AfterClassMethod"/> has been called.
    /// </summary>
    public static int AfterClassCalls;

    /// <summary>
    /// Number of times the per-test <see cref="Before"/> method has been called.
    /// </summary>
    public static int BeforeCalls;

    /// <summary>
    /// Number of times the per-test <see cref="After"/> method has been called.
    /// </summary>
    public static int AfterCalls;

    [BeforeClass]
    public static void BeforeClassMethod()
    {
        Interlocked.Increment(ref BeforeClassCalls);
    }

    /// <summary>
    /// One-time setup method for the entire test class.
    /// Used to verify that <c>BeforeClass</c> is invoked the expected number of times.
    /// </summary>
    [AfterClass]
    public static void AfterClassMethod()
    {
        Interlocked.Increment(ref AfterClassCalls);
    }

    /// <summary>
    /// Per-test setup method that is executed before each test method in this class.
    /// Increments the <see cref="BeforeCalls"/> counter.
    /// </summary>
    [Before]
    public void Before()
    {
        Interlocked.Increment(ref BeforeCalls);
    }

    /// <summary>
    /// Per-test teardown method that is executed after each test method in this class.
    /// Increments the <see cref="AfterCalls"/> counter.
    /// </summary>
    [After]
    public void After()
    {
        Interlocked.Increment(ref AfterCalls);
    }

    /// <summary>
    /// First sample test method used to exercise the lifecycle hooks.
    /// </summary>
    [MyTest]
    public void Test1()
    {
    }

    /// <summary>
    /// Second sample test method used to exercise the lifecycle hooks.
    /// </summary>
    [MyTest]
    public void Test2()
    {
    }
}
