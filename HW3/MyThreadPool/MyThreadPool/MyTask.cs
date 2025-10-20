// <copyright file="MyTask.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Class with my realization of Task.
/// </summary>
/// <typeparam name="TResult">Type that task returns after computation.</typeparam>
public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly Func<TResult> function;

    private readonly Lock locker = new();

    private TResult? result;

    private Exception? exception;

    private bool isCompleted;

    private ManualResetEvent completedEvent = new ManualResetEvent(false);

    private List<Action> continuations = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
    /// </summary>
    /// <param name="function">function for computation.</param>
    public MyTask(Func<TResult> function)
    {
        this.function = function;
    }

    /// <summary>
    /// Gets result of computation of a task function.
    /// </summary>
    public TResult Result
    {
        get
        {
            this.completedEvent.WaitOne();
            if (this.exception != null)
            {
                throw new AggregateException(this.exception);
            }

            return this.result ?? throw new InvalidOperationException("The task has not completed yet.");
        }
    }

    /// <summary>
    /// Gets a value indicating whether true if task is completed, else false.
    /// </summary>
    public bool IsCompleted
    {
        get
        {
            lock (this.locker)
            {
                return this.isCompleted;
            }
        }
    }

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

        lock (this.locker)
        {
            this.isCompleted = true;
        }

        this.completedEvent.Set();

        List<Action> toRun;
        lock (this.locker)
        {
            toRun = new List<Action>(this.continuations);
            this.continuations.Clear();
        }

        foreach (var action in toRun)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error executing task: {e}");
            }
        }
    }

    /// <summary>
    /// Methods for adding continuation functions.
    /// </summary>
    /// <param name="continuationFunction">continuation function.</param>
    /// <typeparam name="TNewResult">type that cont. function returns.</typeparam>
    /// <returns>New IMyTask with cont. function.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuationFunction)
    {
        if (continuationFunction == null)
        {
            throw new ArgumentNullException(nameof(continuationFunction));
        }

        var newTask = new MyTask<TNewResult>(() => continuationFunction(this.Result));

        var runNow = false;
        lock (this.locker)
        {
            runNow = this.isCompleted;
            if (!runNow)
            {
                this.continuations.Add(() => newTask.Execute());
            }
        }

        if (runNow)
        {
            newTask.Execute();
        }

        return newTask;
    }
}
