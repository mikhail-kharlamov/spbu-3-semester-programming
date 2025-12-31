// <copyright file="TestResult.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Models;

/// <summary>
/// DTO that represents a single test result and is convenient for logging and rendering in reports.
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
    /// Fully qualified name of the test including assembly, class and method.
    /// </summary>
    public string FullName => $"{this.AssemblyName}:{this.ClassName}.{this.MethodName}";

    /// <summary>
    /// Short name of the test that includes only class and method.
    /// </summary>
    public string ShortName => $"{this.ClassName}.{this.MethodName}";

    /// <summary>
    /// Indicates whether the test finished successfully.
    /// </summary>
    public bool IsSuccess => this.Status == TestStatus.Passed;
}
