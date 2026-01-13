// <copyright file="MyAssertException.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.MyAssert;

/// <summary>
/// Exception thrown when an assertion fails.
/// </summary>
/// <typeparam name="T">The type of the values being compared.</typeparam>
/// <param name="expected">The expected value.</param>
/// <param name="actual">The actual value found.</param>
public class MyAssertException<T>(T expected, T actual) : Exception
{
    /// <summary>
    /// Gets the expected value.
    /// </summary>
    public T Expected { get; } = expected;

    /// <summary>
    /// Gets the actual value.
    /// </summary>
    public T Actual { get; } = actual;
}
