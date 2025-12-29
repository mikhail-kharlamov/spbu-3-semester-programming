// <copyright file="Homework.cs" company="MyCompany">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>

namespace Homeworks.Database.Models;

public class Homework
{
    public int Id { get; set; }
    
    public DateTime Deadline { get; set; }
    
    public List<CourseTask> Tasks { get; set; } = new();
}
