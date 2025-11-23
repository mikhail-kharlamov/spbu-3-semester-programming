// <copyright file="Test.cs" company="MyCompany">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>

namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class Test : Attribute
{
    public Test(Type? expected = null, string? ignore = null)
    {
        this.Excpected = expected;
        this.Ignore = ignore;
    }
    
    public Type? Excpected { get; }
    
    public string? Ignore { get; }
}