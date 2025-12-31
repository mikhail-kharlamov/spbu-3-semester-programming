// <copyright file="SimpleTests.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>
using MyNUnit.Attributes;

namespace MyNUnit.Tests.ExampleTests;

public class SimpleTests
{
    [MyTest]
    public void Passing()
    {
    }

    [MyTest]
    public void Failing()
    {
        throw new InvalidOperationException("Boom");
    }
}
