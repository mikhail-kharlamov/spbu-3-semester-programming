// <copyright file="BeforeClass.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Attributes;

/// <summary>
/// Marks a method that is executed once before any tests in the class are run.
/// </summary>
/// <remarks>
/// Methods annotated with <see cref="BeforeClass"/> are typically used to perform
/// expensive one-time setup for all tests in the containing class.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class BeforeClass : Attribute;
