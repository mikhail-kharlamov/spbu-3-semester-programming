// <copyright file="MethodsWithAttributes.cs" company="Mikhail Kharlamov">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>

using System.Reflection;

namespace MyNUnit.Models;

public record ClassMethodsByAttributes(
    MethodInfo[] TestMethods,
    MethodInfo[] BeforeClassMethods,
    MethodInfo[] AfterClassMethods,
    MethodInfo[] BeforeMethods,
    MethodInfo[] AfterMethods);
