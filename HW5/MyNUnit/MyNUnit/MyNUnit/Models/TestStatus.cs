// <copyright file="TestStatus.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Models;

/// <summary>
/// Represents the overall status of a single test.
/// </summary>
public enum TestStatus
{
    /// <summary>
    /// The test completed successfully.
    /// </summary>
    Passed,

    /// <summary>
    /// The test failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The test was skipped and not executed.
    /// </summary>
    Ignored,
}
