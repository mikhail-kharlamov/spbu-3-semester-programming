// <copyright file="TestResult.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyNUnit.Web.Database.Models;

public class TestResult
{
    public int Id { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public string MethodName { get; set; } = string.Empty;

    // "Passed", "Failed", "Ignored"
    public string Status { get; set; } = string.Empty;

    // Время выполнения в миллисекундах
    public long DurationMs { get; set; }

    // Сообщение об ошибке или причина Ignore (может быть null, если тест прошел)
    public string? Message { get; set; }
    
    // Стек трейс (может быть null)
    public string? StackTrace { get; set; }

    public int TestAssemblyId { get; set; }
    
    public TestAssembly TestAssembly { get; set; } = null!;
}
