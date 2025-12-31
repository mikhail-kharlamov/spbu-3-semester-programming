// <copyright file="TestRunnerController.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyNUnit.Web.Database;
using MyNUnit.Web.Database.Models;
using DbTestResult = MyNUnit.Web.Database.Models.TestResult;
using Engine = MyNUnit.MyNUnit;
using EngineTestPhase = MyNUnit.Models.TestPhase;
using EngineTestStatus = MyNUnit.Models.TestStatus;

namespace MyNUnitWeb.Api;

/// <summary>
/// API Controller for handling test execution and result retrieval.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TestRunnerController : ControllerBase
{
    private readonly ITestRepository repository;

    private readonly IHostingEnvironment environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestRunnerController"/> class.
    /// </summary>
    /// <param name="repository">Data access repository.</param>
    /// <param name="environment">Hosting environment (used for temp path management).</param>
    public TestRunnerController(ITestRepository repository, IHostingEnvironment environment)
    {
        this.repository = repository;
        this.environment = environment;
    }

    /// <summary>
    /// Uploads assemblies, runs tests, saves results, and returns the test run summary.
    /// </summary>
    /// <param name="files">List of .dll files to test.</param>
    /// <returns>The created TestRun object with results.</returns>
    [HttpPost("run")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> RunTests([FromForm] List<IFormFile> files)
    {
        if (files.Count == 0)
        {
            return this.BadRequest("No assemblies provided.");
        }

        var runId = Guid.NewGuid().ToString();
        var tempPath = Path.Combine(Path.GetTempPath(), "MyNUnit_Uploads", runId);
        Directory.CreateDirectory(tempPath);

        var savedAssemblies = new List<string>();

        try
        {
            foreach (var file in files)
            {
                if (file.Length > 0 && file.FileName.EndsWith(".dll"))
                {
                    var filePath = Path.Combine(tempPath, file.FileName);
                    await using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    savedAssemblies.Add(filePath);
                }
            }

            if (savedAssemblies.Count == 0)
            {
                return this.BadRequest("No valid .dll files found.");
            }

            var testRunModel = await TestRunnerController.ExecuteMyNUnitLogicAsync(tempPath, savedAssemblies);
            await this.repository.SaveTestRunAsync(testRunModel);

            return this.Ok(testRunModel);
        }
        catch (Exception ex)
        {
            return this.StatusCode(500, $"Internal server error: {ex.Message}");
        }
        finally
        {
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath, true);
            }
        }
    }

    /// <summary>
    /// Retrieves the history of all test runs.
    /// </summary>
    /// <returns>List of test runs.</returns>
    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        var history = await this.repository.GetHistoryAsync();
        return this.Ok(history);
    }

    /// <summary>
    /// Retrieves detailed results for a specific assembly.
    /// </summary>
    /// <param name="id">The assembly ID.</param>
    /// <returns>Assembly details with test results.</returns>
    [HttpGet("assembly/{id}")]
    public async Task<IActionResult> GetAssemblyDetails(int id)
    {
        var assembly = await this.repository.GetAssemblyDetailsAsync(id);

        if (assembly == null)
        {
            return this.NotFound($"Assembly with ID {id} not found.");
        }

        return this.Ok(assembly);
    }

    /// <summary>
    /// Executes tests using the MyNUnit engine and maps results to database entities.
    /// </summary>
    private static Task<TestRun> ExecuteMyNUnitLogicAsync(string directoryPath, List<string> assemblyPaths)
    {
        var engineResults = Engine.RunAllTests(directoryPath);

        var testRunEntity = new TestRun
        {
            StartedAt = DateTime.UtcNow,
            Assemblies = new List<TestAssembly>(),
        };

        var groupedByAssembly = engineResults.GroupBy(r => r.AssemblyName);

        foreach (var assemblyGroup in groupedByAssembly)
        {
            var assemblyName = assemblyGroup.Key;

            var assemblyEntity = new TestAssembly
            {
                AssemblyName = assemblyName,
                TestResults = new List<DbTestResult>(),
            };

            foreach (var engineRes in assemblyGroup)
            {
                var statusStr = engineRes.Status switch
                {
                    EngineTestStatus.Passed => "Passed",
                    EngineTestStatus.Failed => "Failed",
                    EngineTestStatus.Ignored => "Ignored",
                    _ => "Unknown",
                };

                var message = engineRes.Message;
                if (engineRes.Phase.HasValue && engineRes.Phase != EngineTestPhase.Test)
                {
                    message = $"[{engineRes.Phase}] {message}";
                }

                if (engineRes.Exception != null && string.IsNullOrEmpty(message))
                {
                    message = engineRes.Exception.Message;
                }

                var resultEntity = new DbTestResult
                {
                    ClassName = engineRes.ClassName,
                    MethodName = engineRes.MethodName,
                    Status = statusStr,
                    DurationMs = (long)engineRes.Duration.TotalMilliseconds,
                    Message = message,
                    StackTrace = engineRes.Exception?.StackTrace,
                };

                assemblyEntity.TestResults.Add(resultEntity);
            }

            assemblyEntity.PassedCount = assemblyEntity.TestResults.Count(r => r.Status == "Passed");
            assemblyEntity.FailedCount = assemblyEntity.TestResults.Count(r => r.Status == "Failed");
            assemblyEntity.IgnoredCount = assemblyEntity.TestResults.Count(r => r.Status == "Ignored");

            testRunEntity.Assemblies.Add(assemblyEntity);
        }

        return Task.FromResult(testRunEntity);
    }
}
