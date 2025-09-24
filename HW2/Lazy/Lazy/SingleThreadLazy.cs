// <copyright file="SingleThreadLazy.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace Lazy;

/// <summary>
/// Class for lazy computation in single-thread mode.
/// </summary>
/// <typeparam name="T">Type that function returns.</typeparam>
public class SingleThreadLazy<T> : ILazy<T>
{
    private Func<T>? supplier;

    private T? value;

    private bool isValueCreated;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleThreadLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier">Function for lazy computation.</param>
    /// <exception cref="ArgumentNullException">Throws if function is null.</exception>
    public SingleThreadLazy(Func<T> supplier)
    {
        this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
    }

    /// <summary>
    /// Method for get the function result value.
    /// </summary>
    /// <returns>Result of function computation.</returns>
    public T Get()
    {
        if (!this.isValueCreated && this.supplier is not null)
        {
            this.value = this.supplier();
            this.supplier = null;
            this.isValueCreated = true;
        }

        if (this.value is null)
        {
            throw new InvalidOperationException("The Lazy has not been initialized.");
        }

        return this.value;
    }
}
