// <copyright file="TestResult.cs" company="Mikhail Kharlamov">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>

namespace MyNUnit.Models;

/// <summary>
/// DTO для одного теста, удобная для логирования и отрисовки.
/// </summary>
public record TestResult(
    string AssemblyName,
    string ClassName,
    string MethodName,
    TestStatus Status,
    TimeSpan Duration,
    TestPhase? Phase = null,
    string? Message = null,
    Exception? Exception = null)
{
    /// <summary>
    /// Полное имя теста для логов и вывода.
    /// </summary>
    public string FullName => $"{AssemblyName}:{ClassName}.{MethodName}";

    /// <summary>
    /// Короткое имя без сборки.
    /// </summary>
    public string ShortName => $"{ClassName}.{MethodName}";

    /// <summary>
    /// Удобный флаг успеха.
    /// </summary>
    public bool IsSuccess => Status == TestStatus.Passed;
}
