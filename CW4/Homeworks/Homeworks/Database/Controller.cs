// <copyright file="Controller.cs" company="MyCompany">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>
using Homeworks.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Homeworks.Database;

/// <summary>
/// Manages database interactions for the homework system, handling the lifecycle of
/// homeworks, tasks, and student solutions.
/// </summary>
public class Controller
{
    /// <summary>
    /// Direct access to the Entity Framework database context.
    /// </summary>
    public readonly AppDbContext Сontext = new AppDbContext();

    /// <summary>
    /// Creates a new homework entry with a specific deadline.
    /// </summary>
    /// <param name="deadline">The cut-off date and time for the homework.</param>
    public void AddHomework(DateTime deadline)
    {
        var utcDeadline = DateTime.SpecifyKind(deadline, DateTimeKind.Utc);
        this.Сontext.Homeworks.Add(new Homework { Deadline = utcDeadline });
        this.Сontext.SaveChanges();
    }

    /// <summary>
    /// Adds a new task to an existing homework.
    /// </summary>
    /// <param name="homeworkId">The ID of the parent homework.</param>
    /// <param name="description">The text description of the task.</param>
    /// <param name="maxScore">The maximum possible score for this task.</param>
    /// <returns>
    /// <c>1</c> if successfully added;
    /// <c>0</c> if the homework ID was not found;
    /// <c>-1</c> if an error occurred during saving.
    /// </returns>
    public int AddTask(int homeworkId, string description, int maxScore)
    {
        var hw = this.Сontext.Homeworks.Find(homeworkId);
        if (hw is null)
        {
            return 0; // no homework with that id
        }

        try
        {
            this.Сontext.Tasks.Add(
                new CourseTask
                {
                    HomeworkId = homeworkId,
                    Description = description,
                    MaxScore = maxScore,
                });
            this.Сontext.SaveChanges();
            return 1; // added
        }
        catch (Exception)
        {
            return -1; // didn't add
        }
    }

    /// <summary>
    /// Deletes a homework and all associated tasks/solutions via cascade delete.
    /// </summary>
    /// <param name="homeworkId">The ID of the homework to remove.</param>
    /// <returns>
    /// <c>1</c> if the deletion was successful;
    /// <c>0</c> if the homework was not found.
    /// </returns>
    public int DeleteHomework(int homeworkId)
    {
        var hw = this.Сontext.Homeworks.Find(homeworkId);
        if (hw is null)
        {
            return 0;
        }

        this.Сontext.Homeworks.Remove(hw);
        this.Сontext.SaveChanges();
        return 1; // success
    }

    /// <summary>
    /// Retrieves all homeworks from the database.
    /// </summary>
    /// <remarks>
    /// Uses eager loading (<c>.Include</c>) to populate the <see cref="Homework.Tasks"/> list
    /// for every homework entity returned.
    /// </remarks>
    /// <returns>A list of homeworks with their related tasks.</returns>
    public List<Homework> ListHomeworks()
    {
        // Подгружаем задачи сразу, чтобы избежать ленивой загрузки в цикле
        var hws = this.Сontext.Homeworks.Include(h => h.Tasks).ToList();
        return hws;
    }

    /// <summary>
    /// Submits a solution for a specific task using the current server time.
    /// </summary>
    /// <param name="taskId">The ID of the task being solved.</param>
    /// <param name="text">The content of the solution.</param>
    /// <returns>
    /// The <see cref="DateTime"/> of submission if successful;
    /// otherwise <c>null</c> if the task ID is invalid.
    /// </returns>
    public DateTime? AddSolution(int taskId, string text)
    {
        var task = this.Сontext.Tasks.Find(taskId);
        if (task is null)
        {
            return null; // Задача не найдена
        }

        var now = DateTime.Now;
        this.Сontext.Solutions.Add(new Solution
        {
            CourseTaskId = taskId,
            Text = text,
            DateSubmitted = now,
            Score = 0,
        });

        this.Сontext.SaveChanges();
        return now;
    }

    /// <summary>
    /// Fetches all solutions for a given task, including nested parent data.
    /// </summary>
    /// <param name="taskId">The ID of the task to inspect.</param>
    /// <remarks>
    /// Includes <c>CourseTask.Homework</c> to allow the caller to compare 
    /// <c>DateSubmitted</c> against the homework's <c>Deadline</c>.
    /// </remarks>
    /// <returns>A list of solutions with populated parent navigation properties.</returns>
    public List<Solution> GetSolutionsForTask(int taskId)
    {
        // Используем this.Context вместо создания нового
        return this.Сontext.Solutions
            .Include(s => s.CourseTask)
            .ThenInclude(t => t.Homework)
            .Where(s => s.CourseTaskId == taskId)
            .ToList();
    }

    /// <summary>
    /// Updates the grade/score for a specific solution.
    /// </summary>
    /// <param name="solutionId">The ID of the solution submission.</param>
    /// <param name="newScore">The new score to assign.</param>
    /// <returns>
    /// <c>true</c> if the update was successful; 
    /// <c>false</c> if the solution ID was not found.
    /// </returns>
    public bool UpdateScore(int solutionId, int newScore)
    {
        var solution = this.Сontext.Solutions.Find(solutionId);
        if (solution == null)
        {
            return false;
        }

        solution.Score = newScore;
        this.Сontext.SaveChanges();
        return true;
    }

    /// <summary>
    /// Calculates the sum of scores across all solutions in the database.
    /// </summary>
    /// <returns>The total accumulated score.</returns>
    public int GetTotalScore()
    {
        return this.Сontext.Solutions.Sum(s => s.Score);
    }
}
