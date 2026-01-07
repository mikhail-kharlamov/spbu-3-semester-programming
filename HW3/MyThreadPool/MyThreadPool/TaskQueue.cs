// <copyright file="TaskQueue.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyThreadPool;

/// <summary>
/// Class for thread-safe queue data structure.
/// </summary>
public class TaskQueue
{
    private readonly Queue<Action> queue = new();
    private bool isShuttingDown = false;

    /// <summary>
    /// Gets length of queue.
    /// </summary>
    public int Count
    {
        get
        {
            lock (this.queue)
            {
                return this.queue.Count;
            }
        }
    }

    /// <summary>
    /// Adds new element to queue.
    /// </summary>
    /// <param name="action">Action to add.</param>
    /// <exception cref="InvalidOperationException">If queue is shutting down.</exception>
    public void Enqueue(Action action)
    {
        lock (this.queue)
        {
            if (this.isShuttingDown)
            {
                throw new InvalidOperationException("The task queue is shutting down.");
            }

            this.queue.Enqueue(action);
            Monitor.Pulse(this.queue);
        }
    }

    /// <summary>
    /// Gets and removes the last element from queue.
    /// </summary>
    /// <returns>The last element from queue or null if shut down.</returns>
    public Action? Dequeue()
    {
        lock (this.queue)
        {
            while (this.queue.Count == 0)
            {
                if (this.isShuttingDown)
                {
                    return null;
                }

                Monitor.Wait(this.queue);
            }

            return this.queue.Dequeue();
        }
    }

    /// <summary>
    /// Method for shutting down queue.
    /// </summary>
    public void Shutdown()
    {
        lock (this.queue)
        {
            this.isShuttingDown = true;
            Monitor.PulseAll(this.queue);
        }
    }
}
