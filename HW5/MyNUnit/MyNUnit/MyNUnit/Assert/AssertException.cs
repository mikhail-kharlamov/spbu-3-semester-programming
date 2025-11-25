// <copyright file="AssertException.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Assert;

public class AssertException<T>(T expected, T actual) : Exception
{
    public T Excpected { get; } = expected;

    public T Actual { get; } = actual;
}
