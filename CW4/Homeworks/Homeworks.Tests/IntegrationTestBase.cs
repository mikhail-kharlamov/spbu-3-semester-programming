// <copyright file="IntegrationTestBase.cs" company="Mikhail Kharlamov">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>

using Homeworks.Database;
using Testcontainers.PostgreSql;

namespace Homeworks.Tests;

/// <summary>
/// Abstract base class for integration tests that require a PostgreSQL database.
/// Manages the lifecycle of a Docker container for the database and provides a fresh context for each test.
/// </summary>
public abstract class IntegrationTestBase
{
    private readonly PostgreSqlContainer postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("test_db")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .Build();

    /// <summary>
    /// Gets the database context for direct verification of data state.
    /// </summary>
    protected AppDbContext Context { get; private set; }

    /// <summary>
    /// Gets the controller instance being tested.
    /// </summary>
    protected Controller Controller { get; private set; }

    /// <summary>
    /// Performs one-time initialization for the entire test assembly/fixture.
    /// Starts the PostgreSQL container and configures environment variables for connection.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        await this.postgresContainer.StartAsync();

        Environment.SetEnvironmentVariable("DB_HOST", this.postgresContainer.Hostname);
        Environment.SetEnvironmentVariable("DB_PORT", this.postgresContainer.GetMappedPublicPort(5432).ToString());
        Environment.SetEnvironmentVariable("DB_NAME", "test_db");
        Environment.SetEnvironmentVariable("DB_USER", "test_user");
        Environment.SetEnvironmentVariable("DB_PASS", "test_password");
        Environment.SetEnvironmentVariable("DB_SCHEMA", "public");
    }

    /// <summary>
    /// Clean up resources after all tests have finished.
    /// Stops and disposes of the Docker container.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        await this.postgresContainer.DisposeAsync();
    }

    /// <summary>
    /// Runs before every individual test method.
    /// Resets the database state to ensure test isolation (Deleted -> Created).
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.Context = new AppDbContext();

        this.Context.Database.EnsureDeleted();
        this.Context.Database.EnsureCreated();

        this.Controller = new Controller();
    }

    /// <summary>
    /// Runs after every individual test method.
    /// Disposes of the database context to free up resources.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this.Context.Dispose();
    }
}
