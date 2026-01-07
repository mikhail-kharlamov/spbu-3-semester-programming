// <copyright file="IMyTask.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Interface for realization of tasks.
/// </summary>
/// <typeparam name="TResult">Type of the task result.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether task is completed.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets the result of task computation.
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// Adds continuation task.
    /// </summary>
    /// <param name="continuationFunction">New function.</param>
    /// <typeparam name="TNewResult">Type that new function returns.</typeparam>
    /// <returns>New task with new result type.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuationFunction);
}
