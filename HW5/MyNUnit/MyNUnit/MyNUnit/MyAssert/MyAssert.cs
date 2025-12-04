// <copyright file="Assert.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.MyAssert;

public static class MyAssert
{
    public static void AreEqual<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new MyAssertException<T>(expected, actual);
        }
    }
}
