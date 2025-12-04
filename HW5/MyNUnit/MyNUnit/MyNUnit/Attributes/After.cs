// <copyright file="After.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Attributes;

/// <summary>
/// Marks a method that is executed after each individual test method in the class.
/// </summary>
/// <remarks>
/// Methods annotated with <see cref="After"/> are usually used to clean up
/// per-test state or resources allocated in <see cref="Before"/> or in the test itself.
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class After : Attribute;
