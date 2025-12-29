// <copyright file="Solution.cs" company="Mikhail Kharlamov">
// Copyright (c) MyCompany. All rights reserved.
// </copyright>

namespace Homeworks.Database.Models;

public class Solution
{
    public int Id { get; set; }
    
    public string Text { get; set; } = string.Empty;
    
    public DateTime DateSubmitted { get; set; }
    
    public int Score { get; set; } = 0;

    public int CourseTaskId { get; set; }
    
    public CourseTask CourseTask { get; set; } = null!;
}
