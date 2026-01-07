// <copyright file="MyTask.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Class with my realization of Task.
/// </summary>
/// <typeparam name="TResult">Type that task returns after computation.</typeparam>
internal class MyTask<TResult> : IMyTask<TResult>
{
    private readonly Func<TResult> function;
    private readonly MyThreadPool threadPool;
    private readonly ManualResetEvent completedEvent = new(false);
    private readonly List<Action> continuations = new();

    private TResult? result;
    private Exception? exception;
    private volatile bool isCompleted;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
    /// </summary>
    /// <param name="function">Function for computation.</param>
    /// <param name="threadPool">Reference to the thread pool.</param>
    public MyTask(Func<TResult> function, MyThreadPool threadPool)
    {
        this.function = function;
        this.threadPool = threadPool;
    }

    /// <inheritdoc/>
    public TResult Result
    {
        get
        {
            this.completedEvent.WaitOne();
            if (this.exception != null)
            {
                throw new AggregateException(this.exception);
            }

            return this.result!;
        }
    }

    /// <inheritdoc/>
    public bool IsCompleted => this.isCompleted;

    /// <summary>
    /// Method for executing task.
    /// </summary>
    public void Execute()
    {
        try
        {
            this.result = this.function();
        }
        catch (Exception e)
        {
            this.exception = e;
        }

        this.isCompleted = true;
        this.completedEvent.Set();

        List<Action> toRun;
        lock (this.continuations)
        {
            toRun = new List<Action>(this.continuations);
            this.continuations.Clear();
        }

        foreach (var action in toRun)
        {
            try
            {
                this.threadPool.EnqueueAction(action);
            }
            catch (InvalidOperationException)
            {
            }
        }
    }

    /// <inheritdoc/>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuationFunction)
    {
        ArgumentNullException.ThrowIfNull(continuationFunction);

        var newTask = new MyTask<TNewResult>(() => continuationFunction(this.Result), this.threadPool);
        var continuationAction = () => newTask.Execute();

        lock (this.continuations)
        {
            if (this.isCompleted)
            {
                this.threadPool.EnqueueAction(continuationAction);
            }
            else
            {
                this.continuations.Add(continuationAction);
            }
        }

        return newTask;
    }
}
