// <copyright file="TestPhase.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Models;

/// <summary>
/// Describes the execution phase in which a test or its infrastructure is being run.
/// </summary>
public enum TestPhase
{
    /// <summary>
    /// One-time setup for the test class, executed before any tests in the class.
    /// </summary>
    BeforeClass,

    /// <summary>
    /// Per-test setup, executed before an individual test method.
    /// </summary>
    Before,

    /// <summary>
    /// The body of the test method itself.
    /// </summary>
    Test,

    /// <summary>
    /// Per-test teardown, executed after an individual test method.
    /// </summary>
    After,

    /// <summary>
    /// One-time teardown for the test class, executed after all tests in the class.
    /// </summary>
    AfterClass,
}
