// <copyright file="CourseTask.cs" company="MyCompany">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>

namespace Homeworks.Database.Models;

public class CourseTask
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int MaxScore { get; set; }

    public int HomeworkId { get; set; }
    public Homework Homework { get; set; } = null!;

    public List<Solution> Solutions { get; set; } = new();
}