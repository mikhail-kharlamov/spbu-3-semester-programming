// <copyright file="MyThreadPool.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Class with my realization of thread pool.
/// </summary>
public class MyThreadPool : IDisposable
{
    private Thread[] threads;

    private TaskQueue queue = new();

    private bool isShutdownInitiated = false;

    private CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// </summary>
    /// <param name="countOfThreads">Count of threads.</param>
    /// <exception cref="ArgumentOutOfRangeException">Throws if countOfThreads less or equals 0.</exception>
    public MyThreadPool(int countOfThreads)
    {
        if (countOfThreads <= 0)
        {
            throw new ArgumentOutOfRangeException("Thread count must be greater than 0");
        }

        this.CountOfThreads = countOfThreads;
        this.threads = new Thread[countOfThreads];

        for (var i = 0; i < this.CountOfThreads; i++)
        {
            this.threads[i] = new Thread(this.Worker);
        }

        foreach (var thread in this.threads)
        {
            thread.Start();
        }
    }

    /// <summary>
    /// Gets count of threads in the tread pool.
    /// </summary>
    public int CountOfThreads { get; }

    /// <summary>
    /// Method that stops thread pool.
    /// </summary>
    public void Shutdown()
    {
        if (this.isShutdownInitiated)
        {
            return;
        }

        this.isShutdownInitiated = true;
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
        if (this.isShutdownInitiated)
        {
            throw new InvalidOperationException("Cannot submit on a shutdown state.");
        }

        var task = new MyTask<TResult>(function);

        this.queue.Enqueue(() => task.Execute());

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
    }

    private void Worker()
    {
        while (!this.isShutdownInitiated)
        {
            if (this.cancellationTokenSource.IsCancellationRequested)
            {
                break;
            }

            var action = this.queue.Dequeue();
            if (action is null)
            {
                break;
            }

            action();
        }
    }
}
