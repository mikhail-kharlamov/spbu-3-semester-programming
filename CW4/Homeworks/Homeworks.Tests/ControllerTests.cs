// <copyright file="ControllerTests.cs" company="Mikhail Kharlamov">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>

namespace Homeworks.Tests;


/// <summary>
/// Integration tests for the <see cref="Homeworks.Database.Controller"/> class.
/// Verifies database operations including CRUD for Homeworks, Tasks, and Solutions.
/// </summary>
[TestFixture]
public class ControllerTests : IntegrationTestBase
{
    /// <summary>
    /// Verifies that <see cref="Homeworks.Database.Controller.AddHomework"/> successfully saves a new homework entity to the database.
    /// </summary>
    [Test]
    public void AddHomework_ShouldCreateRecordInDatabase()
    {
        var deadline = DateTime.UtcNow.AddDays(7);

        this.Controller.AddHomework(deadline);

        var homework = this.Context.Homeworks.FirstOrDefault();
        Assert.That(homework, Is.Not.Null);
        Assert.That(homework.Deadline, Is.EqualTo(deadline).Within(TimeSpan.FromSeconds(1)));
    }

    /// <summary>
    /// Verifies that adding a task to a non-existent homework returns an error code (0) and does not modify the database.
    /// </summary>
    [Test]
    public void AddTask_ShouldFail_IfHomeworkDoesNotExist()
    {
        var result = this.Controller.AddTask(999, "Task Description", 10);

        Assert.That(result, Is.EqualTo(0));
        Assert.That(this.Context.Tasks.Count(), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests the complete lifecycle of a homework assignment:
    /// Creating homework -> Adding a task -> Submitting a solution -> Grading the solution.
    /// </summary>
    [Test]
    public void FullCycle_ShouldWorkCorrectly()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var deadline = DateTime.Now;
        this.Controller.AddHomework(deadline);
        var hwId = this.Context.Homeworks.First().Id;

        var resultTask = this.Controller.AddTask(hwId, "Hard Task", 100);
        Assert.That(resultTask, Is.EqualTo(1));
        var taskId = this.Context.Tasks.First().Id;

        var solutionText = "Console.WriteLine('Hello');";
        var submitDate = this.Controller.AddSolution(taskId, solutionText);
        Assert.That(submitDate, Is.EqualTo(deadline).Within(TimeSpan.FromSeconds(5)));

        var solutions = this.Controller.GetSolutionsForTask(taskId);
        Assert.That(solutions.Count, Is.EqualTo(1));
        Assert.That(solutionText, Is.EqualTo(solutions[0].Text));
        Assert.That(solutions[0].Score, Is.EqualTo(0));

        var solId = solutions[0].Id;
        var updateResult = this.Controller.UpdateScore(solId, 95);
        Assert.That(updateResult, Is.True);

        var freshContext = new Homeworks.Database.AppDbContext();
        var savedScore = freshContext.Solutions.Find(solId)!.Score;
        Assert.That(savedScore, Is.EqualTo(95));
    }

    /// <summary>
    /// Verifies that deleting a homework triggers a cascade delete, removing all associated tasks and solutions.
    /// </summary>
    [Test]
    public void DeleteHomework_ShouldCascadeDeleteTasksAndSolutions()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        this.Controller.AddHomework(DateTime.UtcNow);
        var hwId = this.Context.Homeworks.First().Id;
        this.Controller.AddTask(hwId, "Task", 10);
        var taskId = this.Context.Tasks.First().Id;
        this.Controller.AddSolution(taskId, "Sol");

        Assert.That(this.Context.Solutions.Count(), Is.EqualTo(1));

        this.Controller.DeleteHomework(hwId);

        Assert.That(this.Context.Tasks.Count(), Is.EqualTo(0));
        Assert.That(this.Context.Homeworks.Count(), Is.EqualTo(0));
        Assert.That(this.Context.Solutions.Count(), Is.EqualTo(0));
    }
}
