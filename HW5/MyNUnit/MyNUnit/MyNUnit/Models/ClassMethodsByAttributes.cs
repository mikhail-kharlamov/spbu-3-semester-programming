// <copyright file="ClassMethodsByAttributes.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using System.Reflection;

namespace MyNUnit.Models;

/// <summary>
/// Groups methods of a test class by their MyNUnit lifecycle attributes.
/// </summary>
/// <param name="TestMethods">Methods marked with <see cref="Attributes.MyTest"/>.</param>
/// <param name="BeforeClassMethods">Methods marked with <see cref="Attributes.BeforeClass"/>.</param>
/// <param name="AfterClassMethods">Methods marked with <see cref="Attributes.AfterClass"/>.</param>
/// <param name="BeforeMethods">Methods marked with <see cref="Attributes.Before"/>.</param>
/// <param name="AfterMethods">Methods marked with <see cref="Attributes.After"/>.</param>
public record ClassMethodsByAttributes(
    MethodInfo[] TestMethods,
    MethodInfo[] BeforeClassMethods,
    MethodInfo[] AfterClassMethods,
    MethodInfo[] BeforeMethods,
    MethodInfo[] AfterMethods);
