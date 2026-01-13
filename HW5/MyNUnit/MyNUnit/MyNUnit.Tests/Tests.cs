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
    /// Verifies that <see cref="MyNUnit"/> correctly identifies and executes both passing and failing tests.
    /// </summary>
    /// <remarks>
    /// Ensures that passing tests have <see cref="TestStatus.Passed"/> and failing tests
    /// have <see cref="TestStatus.Failed"/> with the correct exception type.
    /// </remarks>
    [Test]
    public void RunAllTestsExecutesPassingAndFailingTests()
    {
        var results = MyNUnit.RunAllTests(this.assemblyPath);

        var passing = results.Single(
            r =>
            r.ClassName.EndsWith(nameof(SimpleTests)) &&
            r.MethodName == nameof(SimpleTests.Passing));

        Assert.That(passing.Status, Is.EqualTo(TestStatus.Passed));
        Assert.That(passing.Phase, Is.EqualTo(TestPhase.Test));

        var failing = results.Single(
            r =>
            r.ClassName.EndsWith(nameof(SimpleTests)) &&
            r.MethodName == nameof(SimpleTests.Failing));

        Assert.That(failing.Status, Is.EqualTo(TestStatus.Failed));
        Assert.That(failing.Phase, Is.EqualTo(TestPhase.Test));
        Assert.That(failing.Exception, Is.TypeOf<InvalidOperationException>());
    }

    /// <summary>
    /// Verifies that lifecycle methods (BeforeClass, AfterClass, Before, After) are executed in the correct order
    /// and the expected number of times.
    /// </summary>
    [Test]
    public void RunAllTestsRespectsBeforeAfterAndBeforeAfterClass()
    {
        var results = MyNUnit.RunAllTests(this.assemblyPath);

        var classResults = results
            .Where(r => r.ClassName.EndsWith(nameof(LifecycleTests)))
            .ToArray();

        // BeforeClass/AfterClass are no longer reported if successful, so only tests remain
        Assert.That(
            classResults.Count(r => r.Phase == TestPhase.Test),
            Is.EqualTo(2));

        Assert.That(classResults.All(r => r.Status == TestStatus.Passed), Is.True);

        Assert.That(LifecycleTests.BeforeClassCalls, Is.EqualTo(4));
        Assert.That(LifecycleTests.AfterClassCalls, Is.EqualTo(4));
        Assert.That(LifecycleTests.BeforeCalls, Is.EqualTo(8));
        Assert.That(LifecycleTests.AfterCalls, Is.EqualTo(8));
    }

    /// <summary>
    /// Verifies that if a <c>Before</c> method fails, the test is marked as <see cref="TestStatus.Errored"/>.
    /// </summary>
    /// <remarks>
    /// This distinguishes between infrastructure failures (Errored) and assertion failures (Failed).
    /// </remarks>
    [Test]
    public void RunAllTestsReportsFailureInBeforePhase()
    {
        var results = MyNUnit.RunAllTests(this.assemblyPath);

        var r = results.Single(
            r =>
            r.ClassName.EndsWith(nameof(BeforeFailingTests)) &&
            r.MethodName == nameof(BeforeFailingTests.Test1));

        // Now expects Errored due to infrastructure failure
        Assert.That(r.Status, Is.EqualTo(TestStatus.Errored));
        Assert.That(r.Phase, Is.EqualTo(TestPhase.Before));
        Assert.That(r.Message, Does.StartWith("Exception in Before method"));
        Assert.That(r.Exception, Is.TypeOf<InvalidOperationException>());
    }

    /// <summary>
    /// Verifies that providing an invalid path results in a <see cref="FileNotFoundException"/>.
    /// </summary>
    [Test]
    public void RunAllTestsThrowsOnMissingPath()
    {
        var invalidPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        Assert.That(
            () => MyNUnit.RunAllTests(invalidPath),
            Throws.TypeOf<FileNotFoundException>());
    }

    /// <summary>
    /// Verifies that custom assertions from <see cref="MyAssert"/> are correctly handled.
    /// </summary>
    /// <remarks>
    /// Checks that <see cref="MyAssertException{T}"/> triggers a test failure and that
    /// valid assertions result in a passed test.
    /// </remarks>
    [Test]
    public void RunAllTestsCapturesMyAssertFailureAndPassing()
    {
        var results = MyNUnit.RunAllTests(this.assemblyPath);

        var failedResults = results.SingleOrDefault(
            res =>
            res.ClassName.EndsWith(nameof(MyAssertTests)) &&
            res.MethodName == nameof(MyAssertTests.FailingAssert));

        var passedResults = results.SingleOrDefault(
            res =>
                res.ClassName.EndsWith(nameof(MyAssertTests)) &&
                res.MethodName == nameof(MyAssertTests.PassingAssert));

        Assert.That(failedResults, Is.Not.Null);
        Assert.That(failedResults!.Status, Is.EqualTo(TestStatus.Failed));
        Assert.That(failedResults.Phase, Is.EqualTo(TestPhase.Test));
        Assert.That(failedResults.Exception, Is.TypeOf<MyAssertException<int>>());

        Assert.That(passedResults!.Status, Is.EqualTo(TestStatus.Passed));
    }
}
