// <copyright file="AfterClass.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Attributes;

/// <summary>
/// Marks a method that is executed once after all tests in the class have finished.
/// </summary>
/// <remarks>
/// Methods annotated with <see cref="AfterClass"/> are typically used to perform
/// one-time cleanup of shared resources initialized by <see cref="BeforeClass"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class AfterClass : Attribute;
