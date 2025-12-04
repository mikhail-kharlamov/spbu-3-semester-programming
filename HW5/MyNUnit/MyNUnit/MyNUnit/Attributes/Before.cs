// <copyright file="Before.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Attributes;

/// <summary>
/// Marks a method that is executed before each individual test method in the class.
/// </summary>
/// <remarks>
/// Methods annotated with <see cref="Before"/> are usually used to prepare
/// per-test state or fixtures that should be reset before every test.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class Before : Attribute;
