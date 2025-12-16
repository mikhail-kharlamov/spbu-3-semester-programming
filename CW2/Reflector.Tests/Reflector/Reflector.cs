// <copyright file="Reflector.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

using System.Reflection;

namespace Reflector;

/// <summary>
/// ...
/// </summary>
public class Reflector
{
    private readonly BindingFlags all =
        BindingFlags.Instance | BindingFlags.Static |
        BindingFlags.Public | BindingFlags.NonPublic;

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
