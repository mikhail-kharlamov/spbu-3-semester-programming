// <copyright file="MyTest.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Attributes;

/// <summary>
/// Marks a method as a test method for the custom MyNUnit framework.
/// </summary>
/// <remarks>
/// A test method can optionally declare an expected exception type or be marked as ignored
/// with a reason. The framework uses these values to decide whether the test passes,
/// fails, or is skipped.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class MyTest : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyTest"/> class.
    /// </summary>
    /// <param name="expected">
    /// Optional type of the exception that the test is expected to throw.
    /// If specified, the test is considered successful only when an exception
    /// of this type (or a derived type) is thrown.
    /// </param>
    /// <param name="ignore">
    /// Optional reason for ignoring the test. When set, the test is not executed
    /// and is reported as skipped with the provided reason.
    /// </param>
    public MyTest(Type? expected = null, string? ignore = null)
    {
        this.Expected = expected;
        this.Ignore = ignore;
    }

    /// <summary>
    /// Gets the type of the exception that the test is expected to throw, if any.
    /// </summary>
    public Type? Expected { get; }

    /// <summary>
    /// Gets the reason why the test should be ignored, if any.
    /// </summary>
    public string? Ignore { get; }
}
