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
    public int Count => this.queue.Count;

    /// <summary>
    /// Adds new element to queue.
    /// </summary>
    /// <param name="action">.</param>
    /// <exception cref="InvalidOperationException">..</exception>
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
    /// <returns>The last element from queue.</returns>
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

            var result = this.queue.Dequeue();
            return result;
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
