// <copyright file="TestRepository.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using Microsoft.EntityFrameworkCore;
using MyNUnit.Web.Database.Models;

namespace MyNUnit.Web.Database;

/// <summary>
/// Manages database interactions for the MyNUnit system.
/// </summary>
public class TestRepository : ITestRepository
{
    private readonly AppDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestRepository"/> class.
    /// </summary>
    /// <param name="context">The database context injected by the framework.</param>
    public TestRepository(AppDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public async Task<int> SaveTestRunAsync(TestRun testRun)
    {
        // Рассчитываем агрегированную статистику перед сохранением, 
        // если она не была рассчитана вызывающим кодом.
        foreach (var assembly in testRun.Assemblies)
        {
            if (assembly.TestResults.Any() && assembly.PassedCount == 0 && assembly.FailedCount == 0)
            {
                assembly.PassedCount = assembly.TestResults.Count(r => r.Status == "Passed");
                assembly.FailedCount = assembly.TestResults.Count(r => r.Status == "Failed");
                assembly.IgnoredCount = assembly.TestResults.Count(r => r.Status == "Ignored");
            }
        }

        this.context.TestRuns.Add(testRun);
        await this.context.SaveChangesAsync();
        
        return testRun.Id;
    }

    /// <inheritdoc />
    public async Task<List<TestRun>> GetHistoryAsync()
    {
        // Eager loading (Include) только для уровня сборок. 
        // Результаты тестов (TestResults) здесь не грузим — их слишком много.
        return await this.context.TestRuns
            .Include(tr => tr.Assemblies)
            .OrderByDescending(tr => tr.StartedAt)
            .AsNoTracking() // Ускоряет чтение, так как нам не нужно отслеживать изменения
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<TestAssembly?> GetAssemblyDetailsAsync(int assemblyId)
    {
        // Здесь нам нужны детали, поэтому подгружаем TestResults
        return await this.context.TestAssemblies
            .Include(ta => ta.TestResults)
            .Include(ta => ta.TestRun) // Опционально: если нужно показать дату прогона
            .AsNoTracking()
            .FirstOrDefaultAsync(ta => ta.Id == assemblyId);
    }
}
