// <copyright file="Tests.cs" company="Mikhail Kharlamov">
// Copyright (c) Mikhail Kharlamov. All rights reserved.
// </copyright>

namespace MyThreadPool.Tests;

/// <summary>
/// Class with tests for MyThreadPool.
/// </summary>
public class Tests
{
    private MyThreadPool threadPool;

    /// <summary>
    /// Setting up the MyThreadPool object.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this.threadPool = new MyThreadPool(Environment.ProcessorCount);
    }

    /// <summary>
    /// Dispose thread pool.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        this.threadPool.Dispose();
    }

    /// <summary>
    /// Simple test checks that thread pool really works.
    /// </summary>
    [Test]
    public void SubmitAndIsCompletedTest()
    {
        var tasks = new List<IMyTask<int>>();
        var trueResults = new List<int>();

        for (var i = 0; i < 100; i++)
        {
            var localI = i;
            tasks.Add(this.threadPool.Submit(() => localI + localI));
            trueResults.Add(localI + localI);
        }

        for (var i = 0; i < tasks.Count; i++)
        {
            Assert.That(tasks[i].Result, Is.EqualTo(trueResults[i]));
            Assert.That(tasks[i].IsCompleted, Is.True);
        }
    }

    /// <summary>
    /// Test for ContinueWith method for MyTask.
    /// </summary>
    [Test]
    public void ContinueWithAndIsCompletedTest()
    {
        var task = this.threadPool.Submit(() => 1);
        Assert.That(task.Result, Is.EqualTo(1));
        Assert.That(task.IsCompleted, Is.True);

        var newTask = task.ContinueWith(x => x + 1);
        Assert.That(newTask.Result, Is.EqualTo(2));
        Assert.That(newTask.IsCompleted, Is.True);

        var newTask2 = newTask.ContinueWith(x => x + 1);
        Assert.That(newTask2.Result, Is.EqualTo(3));
        Assert.That(newTask2.IsCompleted, Is.True);
    }

    /// <summary>
    /// Checks that shutting down thread pool works.
    /// </summary>
    [Test]
    public void ShutdownTest()
    {
        var executedTasks = 0;
        var totalTasks = Environment.ProcessorCount * 2;

        for (var i = 0; i < totalTasks; i++)
        {
            this.threadPool.Submit(
                () =>
                {
                    Thread.Sleep(100);
                    Interlocked.Increment(ref executedTasks);
                    return 1;
                });
        }

        Thread.Sleep(100);
        this.threadPool.Shutdown();
        Assert.That(executedTasks, Is.LessThan(totalTasks));
    }
}
