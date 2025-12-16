// <copyright file="Reflector.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using System.Reflection;

namespace Reflector;

/// <summary>
/// Provides simple reflection-based utilities for inspecting and comparing types.
/// The <see cref="Reflector"/> class offers two main operations:
/// 1. <see cref="PrintStructure(Type)"/> — generates a minimal C#-style class
///    declaration for a given <see cref="Type"/> and writes it to a file.
/// 2. <see cref="DiffClasses(Type, Type)"/> — compares the fields of two types
///    and prints the differences to the console.
/// </summary>
public class Reflector
{
    private readonly BindingFlags all =
        BindingFlags.Instance | BindingFlags.Static |
        BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// Generates a basic C#-like class declaration for the specified type
    /// and appends it to a file named after the type (e.g. "MyType.cs").
    /// The generated output includes:
    /// - A single <c>public class</c> declaration with the type name.
    /// - All fields obtained via reflection using instance/static and
    ///   public/non-public <see cref="BindingFlags"/>.
    /// - Simple access modifiers (<c>public</c> or <c>private</c>) and
    ///   the <c>static</c> keyword when applicable.
    /// - For static fields, the current field value obtained via
    ///   <see cref="FieldInfo.GetValue(object?)"/>; for instance fields,
    ///   a placeholder initializer <c>= null!;</c> is used instead of
    ///   reading a real instance value.
    /// The method appends to the file if it already exists.
    /// It does not attempt to create or manage an instance of the type.
    /// </summary>
    /// <param name="someClass">The type to inspect and render as a class definition.</param>
    public void PrintStructure(Type someClass)
    {
        var path = $"{someClass.Name}.cs";

        var text = new List<string>();
        text.Add($"public class {someClass.Name}");
        text.Add("{");
        foreach (var field in someClass.GetFields(this.all))
        {
            var value = field.IsStatic
                ? $" = {field.GetValue(null)};"
                : " = null!;";
            text.Add($"    {this.GetKeyWords(field)}{field.Name}{value}");
        }

        text.Add("}");

        using var writer = new StreamWriter(path, append: true);
        foreach (var line in text)
        {
            writer.WriteLine(line);
        }
    }

    /// <summary>
    /// Compares the fields of two types and prints which field names
    /// are present in one type but missing in the other.
    /// The comparison is performed by:
    /// - Retrieving all fields of each type with the same <see cref="BindingFlags"/>
    ///   used elsewhere in this class.
    /// - Extracting their names and computing the set difference in both directions.
    /// - Writing a simple human-readable diff to the console, showing:
    ///   - Fields in <paramref name="a"/> that are not present in <paramref name="b"/>.
    ///   - Fields in <paramref name="b"/> that are not present in <paramref name="a"/>.
    /// The method currently only prints field differences; properties are retrieved
    /// but not processed or output.
    /// </summary>
    /// <param name="a">The first type to compare.</param>
    /// <param name="b">The second type to compare.</param>
    public void DiffClasses(Type a, Type b)
    {
        var firstFields = a.GetFields(this.all);
        var secondFields = b.GetFields(this.all);

        var firstFieldsNames = firstFields.Select(x => x.Name).ToArray();
        var secondFieldsNames = secondFields.Select(x => x.Name).ToArray();

        Console.WriteLine($"{a.Name} fields that not in {b.Name} fields:");
        foreach (var fieldName in firstFieldsNames)
        {
            if (!secondFieldsNames.Contains(fieldName))
            {
                Console.WriteLine($"    {fieldName};");
            }
        }

        Console.WriteLine($"{b.Name} fields that not in {a.Name} fields:");
        foreach (var fieldName in secondFieldsNames)
        {
            if (!firstFieldsNames.Contains(fieldName))
            {
                Console.WriteLine($"    {fieldName};");
            }
        }

        var firstProperties = a.GetProperties(this.all);
        var secondFieldsProperties = b.GetProperties(this.all);
    }

    private string GetKeyWords(FieldInfo field)
    {
        var result = string.Empty;

        if (field.IsPrivate)
        {
            result += "private ";
        }
        else
        {
            result += "public ";
        }

        if (field.IsStatic)
        {
            result += "static ";
        }

        result += field.FieldType.Name + " ";

        return result;
    }
}
