// <copyright file="TestRun.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Web.Database.Models;

public class TestRun
{
    public int Id { get; set; }

    public DateTime StartedAt { get; set; }

    public List<TestAssembly> Assemblies { get; set; } = new();
}
