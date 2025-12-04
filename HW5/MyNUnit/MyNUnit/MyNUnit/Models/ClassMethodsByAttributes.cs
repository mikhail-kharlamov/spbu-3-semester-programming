// <copyright file="ClassMethodsByAttributes.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using System.Reflection;

namespace MyNUnit.Models;

public record ClassMethodsByAttributes(
    MethodInfo[] TestMethods,
    MethodInfo[] BeforeClassMethods,
    MethodInfo[] AfterClassMethods,
    MethodInfo[] BeforeMethods,
    MethodInfo[] AfterMethods);
