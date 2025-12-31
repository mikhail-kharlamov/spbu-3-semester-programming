// <copyright file="Tests.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using MyNUnit.Models;
using MyNUnit.MyAssert;
using MyNUnit.Tests.ExampleTests;

namespace MyNUnit.Tests;

/// <summary>
/// Integration tests for the custom MyNUnit framework.
/// </summary>
/// <remarks>
/// These tests use the real MyNUnit runner against the current test assembly
/// to verify discovery, execution pipeline, lifecycle methods and assertion handling.
/// </remarks>
[TestFixture]
public class Tests
{
    private readonly string assemblyPath = typeof(Tests).Assembly.Location;

    /// <summary>
    /// Verifies that <see cref="MyNUnit.MyNUnit"/> executes both passing and failing tests
    /// and correctly populates <see cref="TestResult"/> for each case.
    /// </summary>
    [Test]
    public void RunAllTestsExecutesPassingAndFailingTests()
    {
        var runner = new MyNUnit();
        var results = runner.RunAllTests(this.assemblyPath);

        var passing = results.Single(
            r =>
            r.ClassName.EndsWith(nameof(SimpleTests)) &&
            r.MethodName == nameof(SimpleTests.Passing));

        Assert.That(passing.Status, Is.EqualTo(Models.TestStatus.Passed));
        Assert.That(passing.Phase, Is.EqualTo(TestPhase.Test));
        Assert.That(passing.Exception, Is.Null);

        var failing = results.Single(
            r =>
            r.ClassName.EndsWith(nameof(SimpleTests)) &&
            r.MethodName == nameof(SimpleTests.Failing));

        Assert.That(failing.Status, Is.EqualTo(TestStatus.Failed));
        Assert.That(failing.Phase, Is.EqualTo(TestPhase.Test));
        Assert.That(failing.Exception, Is.TypeOf<InvalidOperationException>());
    }

    /// <summary>
    /// Verifies that class and per-test lifecycle methods
    /// (<c>BeforeClass</c>, <c>AfterClass</c>, <c>Before</c>, <c>After</c>)
    /// are invoked the expected number of times and all lifecycle-driven tests pass.
    /// </summary>
    [Test]
    public void RunAllTestsRespectsBeforeAfterAndBeforeAfterClass()
    {
        var runner = new MyNUnit();
        var results = runner.RunAllTests(this.assemblyPath);

        var classResults = results
            .Where(r => r.ClassName.EndsWith(nameof(LifecycleTests)))
            .ToArray();

        Assert.That(
            classResults.Count(r => r.Phase == TestPhase.Test),
            Is.EqualTo(2));

        Assert.That(classResults.All(r => r.Status == TestStatus.Passed), Is.True);

        Assert.That(LifecycleTests.BeforeClassCalls, Is.EqualTo(3));
        Assert.That(LifecycleTests.AfterClassCalls, Is.EqualTo(3));
        Assert.That(LifecycleTests.BeforeCalls, Is.EqualTo(6));
        Assert.That(LifecycleTests.AfterCalls, Is.EqualTo(6));
    }

    /// <summary>
    /// Verifies that failures occurring in <c>Before</c> methods
    /// are reported with <see cref="TestPhase.Before"/> and an appropriate message.
    /// </summary>
    [Test]
    public void RunAllTestsReportsFailureInBeforePhase()
    {
        var runner = new MyNUnit();
        var results = runner.RunAllTests(this.assemblyPath);

        var r = results.Single(
            r =>
            r.ClassName.EndsWith(nameof(BeforeFailingTests)) &&
            r.MethodName == nameof(BeforeFailingTests.Test1));

        Assert.That(r.Status, Is.EqualTo(Models.TestStatus.Failed));
        Assert.That(r.Phase, Is.EqualTo(TestPhase.Before));
        Assert.That(r.Message, Does.StartWith("Exception in Before method"));
        Assert.That(r.Exception, Is.TypeOf<InvalidOperationException>());
    }

    /// <summary>
    /// Verifies that <see cref="MyNUnit.MyNUnit.RunAllTests(string)"/> throws
    /// <see cref="FileNotFoundException"/> when the provided path does not exist.
    /// </summary>
    [Test]
    public void RunAllTestsThrowsOnMissingPath()
    {
        var runner = new MyNUnit();
        var invalidPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        Assert.That(
            () => runner.RunAllTests(invalidPath),
            Throws.TypeOf<FileNotFoundException>());
    }

    /// <summary>
    /// Verifies that failures produced by <see cref="MyAssert.AreEqual{T}(T,T)"/>
    /// are captured by MyNUnit as failed tests with <see cref="MyAssertException{T}"/>,
    /// and that passing assertions are reported as successful tests.
    /// </summary>
    [Test]
    public void RunAllTestsCapturesMyAssertFailureAndPassing()
    {
        var runner = new MyNUnit();
        var results = runner.RunAllTests(this.assemblyPath);

        var failedResults = results.SingleOrDefault(
            res =>
            res.ClassName.EndsWith(nameof(MyAssertTests)) &&
            res.MethodName == nameof(MyAssertTests.FailingAssert));

        var passedResults = results.SingleOrDefault(
            res =>
                res.ClassName.EndsWith(nameof(MyAssertTests)) &&
                res.MethodName == nameof(MyAssertTests.PassingAssert));

        Assert.That(failedResults, Is.Not.Null, "Test result for MyAssertSampleTests.FailingAssert not found");
        Assert.That(failedResults!.Status, Is.EqualTo(TestStatus.Failed));
        Assert.That(failedResults.Phase, Is.EqualTo(TestPhase.Test));
        Assert.That(failedResults.Exception, Is.TypeOf<MyAssertException<int>>());

        var ex = (MyAssertException<int>)failedResults.Exception!;
        Assert.That(ex.Excpected, Is.EqualTo(1));
        Assert.That(ex.Actual, Is.EqualTo(2));

        Assert.That(passedResults!.Status, Is.EqualTo(TestStatus.Passed));
        Assert.That(passedResults.Phase, Is.EqualTo(TestPhase.Test));
    }
}
