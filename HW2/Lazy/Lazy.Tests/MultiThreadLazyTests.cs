namespace Lazy;

/// <summary>
/// Class with tests for thread races in lazy multithread mode.
/// </summary>
public class MultiThreadLazyTests
{
    /// <summary>
    /// Checks that supplier called only once with thread races.
    /// </summary>
    [Test]
    public void SupplierCalledOnlyOnceInMultithreadedEnvironment()
    {
        int counter = 0;
        Func<int> supplier = () =>
        {
            Interlocked.Increment(ref counter);
            Thread.Sleep(50);
            return 42;
        };

        var lazy = new MultiThreadLazy<int>(supplier);

        var results = new int[20];
        List<Thread> threads = new();
        for (var i = 0; i < 20; i++)
        {
            var index = i;
            var thread = new Thread(
                () =>
                {
                results[index] = lazy.Get();
            });
            threads.Add(thread);
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        foreach (var result in results)
        {
            Assert.That(result, Is.EqualTo(42));
        }

        Assert.That(counter, Is.EqualTo(1), "Supplier must be called exactly once.");
    }
}
