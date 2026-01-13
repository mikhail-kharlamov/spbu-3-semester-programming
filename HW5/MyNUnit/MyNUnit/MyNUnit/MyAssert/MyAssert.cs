// <copyright file="MyAssert.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.MyAssert;

/// <summary>
/// Provides static methods to test conditions (assertions).
/// </summary>
public static class MyAssert
{
    /// <summary>
    /// Verifies that two specified objects are equal.
    /// </summary>
    /// <typeparam name="T">The type of the objects to compare.</typeparam>
    /// <param name="expected">The value expected to be found.</param>
    /// <param name="actual">The value actually produced by the operation.</param>
    /// <exception cref="MyAssertException{T}">Thrown when <paramref name="expected"/> does not equal <paramref name="actual"/>.</exception>
    public static void AreEqual<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new MyAssertException<T>(expected, actual);
        }
    }
}
