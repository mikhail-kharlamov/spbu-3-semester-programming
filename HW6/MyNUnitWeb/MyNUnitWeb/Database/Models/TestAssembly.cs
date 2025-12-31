// <copyright file="TestAssembly.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Web.Database.Models;

public class TestAssembly
{
    public int Id { get; set; }

    public string AssemblyName { get; set; } = string.Empty;

    public int PassedCount { get; set; }
    
    public int FailedCount { get; set; }
    
    public int IgnoredCount { get; set; }

    public int TestRunId { get; set; }
    
    public TestRun TestRun { get; set; } = null!;

    public List<TestResult> TestResults { get; set; } = new();
}
