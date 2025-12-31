// <copyright file="BeforeFailingTests.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>
using MyNUnit.Attributes;

namespace MyNUnit.Tests.ExampleTests;

/// <summary>
/// Sample test class used to verify that failures in <c>Before</c> methods
/// are reported by the MyNUnit framework as failures in the <see cref="TestPhase.Before"/> phase.
/// </summary>
public class BeforeFailingTests
{
    /// <summary>
    /// Per-test setup method that always throws an exception to simulate
    /// a failure during the <c>Before</c> phase.
    /// </summary>
    [Before]
    public void Before()
    {
        throw new InvalidOperationException("Fail in Before");
    }

    /// <summary>
    /// Test method that should never be executed because the <c>Before</c> method fails.
    /// </summary>
    [MyTest]
    public void Test1()
    {
    }
}
