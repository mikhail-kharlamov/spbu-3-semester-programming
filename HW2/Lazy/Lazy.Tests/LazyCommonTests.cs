namespace Lazy;

/// <summary>
/// ...
/// </summary>
/// <typeparam name="T">..</typeparam>
[TestFixture(typeof(SingleThreadLazy<int>))]
[TestFixture(typeof(MultiThreadLazy<int>))]
public class LazyCommonTests<T>
    where T : ILazy<int>
{
    /// <summary>
    /// ..
    /// </summary>
    [Test]
    public void SupplierCalledOnce()
    {
        var counter = 0;
        var supplier = () =>
        {
            counter++;
            return 42;
        };

        var lazy = this.CreateLazy(supplier);
        var first = lazy.Get();
        var second = lazy.Get();
        var third = lazy.Get();

        Assert.That(counter, Is.EqualTo(1));
        Assert.That(first, Is.EqualTo(42));
        Assert.That(second, Is.EqualTo(42));
        Assert.That(third, Is.EqualTo(42));
    }

    /// <summary>
    /// ..
    /// </summary>
    [Test]
    public void SupplierNotCalledBeforeGet()
    {
        var called = false;
        var supplier = () =>
        {
            called = true;
            return 42;
        };

        var lazy = this.CreateLazy(supplier);
        Assert.That(called, Is.False);
        lazy.Get();
        Assert.That(called, Is.True);
    }

    private ILazy<int> CreateLazy(Func<int> supplier)
    {
        if (typeof(T) == typeof(SingleThreadLazy<int>))
        {
            return new SingleThreadLazy<int>(supplier);
        }

        if (typeof(T) == typeof(MultiThreadLazy<int>))
        {
            return new MultiThreadLazy<int>(supplier);
        }

        throw new InvalidOperationException("Unsupported Lazy type");
    }
}
