// <copyright file="Size.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace BenchmarkRunner;

/// <summary>
/// Represents a two-dimensional matrix size.
/// </summary>
/// <param name="Width">Number of columns.</param>
/// <param name="Height">Number of rows.</param>
public record Size(int Width, int Height);
