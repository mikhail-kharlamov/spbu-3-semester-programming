// <copyright file="MyThreadPool.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Class with my realization of thread pool.
/// </summary>
public class MyThreadPool : IDisposable
{
    private readonly Thread[] threads;
    private readonly TaskQueue queue = new();
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly Lock shutdownLock = new();
    private volatile bool isShutdownInitiated;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="countOfThreads">Count of threads.</param>
    /// <exception cref="ArgumentOutOfRangeException">Throws if countOfThreads less or equals 0.</exception>
    public MyThreadPool(int countOfThreads)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(countOfThreads);

        this.threads = new Thread[countOfThreads];

        for (var i = 0; i < countOfThreads; i++)
        {
            this.threads[i] = new Thread(this.Worker);
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Method that stops thread pool.
    /// </summary>
    public void Shutdown()
    {
        lock (this.shutdownLock)
        {
            if (this.isShutdownInitiated)
            {
                return;
            }

            this.isShutdownInitiated = true;
        }

        this.cancellationTokenSource.Cancel();
        this.queue.Shutdown();

        foreach (var thread in this.threads)
        {
            thread.Join();
        }
    }

    /// <summary>
    /// Method for adding new task.
    /// </summary>
    /// <param name="function">Function for computing.</param>
    /// <typeparam name="TResult">Param that function returns.</typeparam>
    /// <exception cref="InvalidOperationException">Throws if shutdown initiated.</exception>
    /// <returns>MyTask object for entering function.</returns>>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        var task = new MyTask<TResult>(function, this);
        this.EnqueueAction(() => task.Execute());
        return task;
    }

    /// <summary>
    /// Implements method from IDisposable interface.
    /// </summary>
    public void Dispose()
    {
        if (!this.isShutdownInitiated)
        {
            this.Shutdown();
        }

        this.cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Internal method to enqueue raw actions (used by continuations).
    /// </summary>
    /// <param name="action">Action to execute.</param>
    internal void EnqueueAction(Action action) => this.queue.Enqueue(action);

    private void Worker()
    {
        while (true)
        {
            var action = this.queue.Dequeue();
            if (action is null)
            {
                break;
            }

            action();
        }
    }
}
