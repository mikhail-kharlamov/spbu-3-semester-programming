// <copyright file="ITestRepository.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using MyNUnit.Web.Database.Models;

namespace MyNUnit.Web.Database;

/// <summary>
/// Defines the contract for data access operations related to test runs and results.
/// </summary>
public interface ITestRepository
{
    /// <summary>
    /// Saves a complete test run with all its assemblies and results to the database.
    /// </summary>
    /// <param name="testRun">The populated test run entity.</param>
    /// <returns>The ID of the created test run.</returns>
    Task<int> SaveTestRunAsync(TestRun testRun);

    /// <summary>
    /// Retrieves the history of all test runs without loading detailed test results.
    /// </summary>
    /// <returns>A list of test runs ordered by date descending.</returns>
    Task<List<TestRun>> GetHistoryAsync();

    /// <summary>
    /// Retrieves a specific assembly with all its individual test results.
    /// </summary>
    /// <param name="assemblyId">The ID of the assembly.</param>
    /// <returns>The assembly with populated results, or null if not found.</returns>
    Task<TestAssembly?> GetAssemblyDetailsAsync(int assemblyId);
}
