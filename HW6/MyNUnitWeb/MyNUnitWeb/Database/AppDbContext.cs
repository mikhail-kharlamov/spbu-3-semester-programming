// <copyright file="AppDbContext.cs" company="Mikhail Kharlamov">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>
using MyNUnit.Web.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace MyNUnit.Web.Database;

/// <summary>
/// Represents the primary database session for the application, defining the mapping 
/// between C# entities and PostgreSQL database tables.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Gets or sets the data set for homework assignments.
    /// </summary>
    public DbSet<TestAssembly> TestAssemblies { get; set; }

    /// <summary>
    /// Gets or sets the data set for individual tasks within assignments.
    /// </summary>
    public DbSet<TestResult> TestResults { get; set; }

    /// <summary>
    /// Gets or sets the data set for individual tasks within assignments.
    /// </summary>
    public DbSet<TestRun> TestRuns { get; set; }

    /// <summary>
    /// Configures the database connection using environment variables.
    /// </summary>
    /// <param name="optionsBuilder">The builder used to configure the context options.</param>
    /// <remarks>
    /// This method expects the following environment variables to be set:
    /// <list type="bullet">
    /// <item><c>DB_HOST</c>: The database server address.</item>
    /// <item><c>DB_PORT</c>: The port number.</item>
    /// <item><c>DB_NAME</c>: The database name.</item>
    /// <item><c>DB_USER</c>: The username.</item>
    /// <item><c>DB_PASS</c>: The password.</item>
    /// <item><c>DB_SCHEMA</c>: The target schema (defaults to "public" if not set).</item>
    /// </list>
    /// It configures Npgsql with the <c>SearchPath</c> parameter to simplify schema usage.
    /// </remarks>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var db = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var pass = Environment.GetEnvironmentVariable("DB_PASS");
        var schema = Environment.GetEnvironmentVariable("DB_SCHEMA") ?? "public";

        var connectionString =
            $"Host={host};Port={port};Database={db};Username={user};Password={pass};SearchPath={schema};";

        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var schema = Environment.GetEnvironmentVariable("DB_SCHEMA") ?? "public";

        modelBuilder.HasDefaultSchema(schema);

        base.OnModelCreating(modelBuilder);
    }
}
