// <copyright file="IMyTask.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Interface for realization of tasks.
/// </summary>
/// <typeparam name="TResult">..</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether true if task is completed else false.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets a value indicating whether result of task computation.
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// Adds next functions for task.
    /// </summary>
    /// <param name="continuationFunction">New function.</param>
    /// <typeparam name="TNewResult">Type that new function returns.</typeparam>
    /// <returns>New task with new result type.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuationFunction);
}
