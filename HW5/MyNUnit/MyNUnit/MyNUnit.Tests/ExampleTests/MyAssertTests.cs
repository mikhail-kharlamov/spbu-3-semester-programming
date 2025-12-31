// <copyright file="MyAssertTests.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>
using MyNUnit.Attributes;

namespace MyNUnit.Tests.ExampleTests;

/// <summary>
/// Sample tests that exercise the custom <c>MyAssert</c> assertion helpers
/// and verify how MyNUnit reports assertion failures and successes.
/// </summary>
public class MyAssertTests
{
    /// <summary>
    /// Test that intentionally fails by comparing different integer values,
    /// causing <see cref="MyAssert.MyAssert.AreEqual{T}(T,T)"/> to throw
    /// <see cref="MyNUnit.MyAssert.MyAssertException{T}"/>.
    /// </summary>
    [MyTest]
    public void FailingAssert()
    {
        MyAssert.MyAssert.AreEqual(1, 2);
    }

    [MyTest]
    public void PassingAssert()
    {
        MyAssert.MyAssert.AreEqual("11", "11");
    }
}
