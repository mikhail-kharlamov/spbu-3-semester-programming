// <copyright file="Program.cs" company="Mikhail Kharlamov">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>

using DotNetEnv;
using Homeworks.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure; // Для GetService
using Microsoft.EntityFrameworkCore.Storage;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

Env.TraversePath().Load();

var controller = new Controller();

try
{
    var schema = Environment.GetEnvironmentVariable("DB_SCHEMA") ?? "public";

    controller.Сontext.Database.ExecuteSql($"DROP SCHEMA IF EXISTS \"{schema}\" CASCADE;");

    controller.Сontext.Database.ExecuteSql($"CREATE SCHEMA IF NOT EXISTS \"{schema}\";");

    controller.Сontext.Database.GetService<IRelationalDatabaseCreator>();

    var databaseCreator = controller.Сontext.Database.GetService<IRelationalDatabaseCreator>();
    databaseCreator.CreateTables();

    Console.WriteLine($"[DEBUG] Таблицы успешно созданы в схеме: {schema}");
}
catch (Exception ex)
{
    Console.WriteLine($"Critical database error: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Детали ошибки: {ex.InnerException.Message}");
    }

    return;
}

try
{
    controller.Сontext.Database.EnsureCreated();
}
catch (Exception ex)
{
    Console.WriteLine($"Database connection error: {ex.Message}");
    return;
}

Console.WriteLine("=== Homework Management System ===");

while (true)
{
    Console.WriteLine("\n-------------------------------------");
    Console.WriteLine("1. Add Homework");
    Console.WriteLine("2. Add Task to Homework");
    Console.WriteLine("3. Delete Homework");
    Console.WriteLine("4. List All Homeworks and Tasks");
    Console.WriteLine("5. Submit Solution");
    Console.WriteLine("6. View/Review Solutions");
    Console.WriteLine("7. Grade Solution");
    Console.WriteLine("8. Show Total Score");
    Console.WriteLine("0. Exit");
    Console.WriteLine("-------------------------------------");
    Console.Write("> ");

    var input = Console.ReadLine();

    switch (input)
    {
        case "1": AddHomeworkUi(); break;
        case "2": AddTaskUi(); break;
        case "3": DeleteHomeworkUi(); break;
        case "4": ListAllUi(); break;
        case "5": SubmitSolutionUi(); break;
        case "6": ViewSolutionsUi(); break;
        case "7": RateSolutionUi(); break;
        case "8": ShowTotalScoreUi(); break;
        case "0": return;
        default: Console.WriteLine("Unknown command."); break;
    }
}


void AddHomeworkUi()
{
    Console.Write("Enter deadline (format YYYY-MM-DD HH:MM): ");
    var dateStr = Console.ReadLine();

    if (DateTime.TryParse(dateStr, out DateTime dt))
    {
        controller.AddHomework(dt);
        Console.WriteLine($"[OK] Homework created with deadline {dt}");
    }
    else
    {
        Console.WriteLine("[!] Invalid date format.");
    }
}

void AddTaskUi()
{
    var hwId = ReadInt("Homework ID");
    Console.Write("Task description: ");
    var desc = Console.ReadLine() ?? "No description";
    var maxScore = ReadInt("Max score");

    var result = controller.AddTask(hwId, desc, maxScore);

    switch (result)
    {
        case 1:
            Console.WriteLine("[OK] Task successfully added.");
            break;
        case 0:
            Console.WriteLine("[!] Error: Homework with that ID not found.");
            break;
        default:
            Console.WriteLine("[!] Database error during save.");
            break;
    }
}

void DeleteHomeworkUi()
{
    var hwId = ReadInt("Homework ID to delete");
    var result = controller.DeleteHomework(hwId);

    if (result == 1)
    {
        Console.WriteLine($"[OK] Homework #{hwId} and all related data deleted.");
    }
    else
    {
        Console.WriteLine($"[!] Homework #{hwId} not found.");
    }
}

void ListAllUi()
{
    var list = controller.ListHomeworks();

    if (list.Count == 0)
    {
        Console.WriteLine("List is empty.");
        return;
    }

    foreach (var hw in list)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n[HW #{hw.Id}] Deadline: {hw.Deadline:f}");
        Console.ResetColor();

        if (hw.Tasks.Count == 0)
        {
            Console.WriteLine("   (no tasks)");
        }
        else
        {
            foreach (var t in hw.Tasks)
            {
                Console.WriteLine($"   - Task #{t.Id}: {t.Description} (max: {t.MaxScore})");
            }
        }
    }
}

void SubmitSolutionUi()
{
    var taskId = ReadInt("Task ID");
    Console.Write("Your solution text: ");
    var text = Console.ReadLine() ?? string.Empty;

    var submittedDate = controller.AddSolution(taskId, text);

    if (submittedDate.HasValue)
    {
        Console.WriteLine($"[OK] Solution submitted at {submittedDate.Value:T}.");
    }
    else
    {
        Console.WriteLine("[!] Error: Task with that ID does not exist.");
    }
}

void ViewSolutionsUi()
{
    var taskId = ReadInt("Task ID");
    var solutions = controller.GetSolutionsForTask(taskId);

    if (solutions.Count == 0)
    {
        Console.WriteLine("No solutions found.");
        return;
    }

    foreach (var s in solutions)
    {
        var deadline = s.CourseTask.Homework.Deadline;
        var isLate = s.DateSubmitted > deadline;

        var statusTag = isLate ? "[LATE]" : "[OK]";
        var color = isLate ? ConsoleColor.Red : ConsoleColor.Green;

        Console.Write($"Sol #{s.Id} ");

        Console.ForegroundColor = color;
        Console.Write(statusTag);
        Console.ResetColor();

        Console.WriteLine($" {s.DateSubmitted:g} | Score: {s.Score}");
        Console.WriteLine($"Text: {s.Text}");
        Console.WriteLine("-");
    }
}

void RateSolutionUi()
{
    var solId = ReadInt("Solution ID");
    var score = ReadInt("New score");

    var success = controller.UpdateScore(solId, score);

    if (success)
    {
        Console.WriteLine("[OK] Score updated.");
    }
    else
    {
        Console.WriteLine("[!] Solution not found.");
    }
}

void ShowTotalScoreUi()
{
    var total = controller.GetTotalScore();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n>>> YOUR TOTAL SCORE: {total} POINTS <<<");
    Console.ResetColor();
}

int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write($"{prompt}: ");
        if (int.TryParse(Console.ReadLine(), out var result))
        {
            return result;
        }

        Console.WriteLine("Please enter a valid number.");
    }
}
