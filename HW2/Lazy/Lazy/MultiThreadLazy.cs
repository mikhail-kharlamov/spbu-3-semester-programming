namespace Lazy;

/// <summary>
/// Class for lazy computation in single-thread mode.
/// </summary>
/// <param name="supplier">Function object for computation.</param>
/// <typeparam name="T">Type that function returns.</typeparam>
public class MultiThreadLazy<T>(Func<T> supplier) : ILazy<T>
{
    private readonly Lock locker = new Lock();

    private Func<T>? supplier = supplier;

    private T value;

    private bool isValueCreated = false;

    /// <summary>
    /// Method for get the function resuilt value.
    /// </summary>
    /// <returns>Result of function computation.</returns>
    public T Get()
    {
        if (this.isValueCreated)
        {
            return this.value;
        }

        lock (this.locker)
        {
            if (!this.isValueCreated)
            {
                this.value = this.supplier();
                this.supplier = null;
                this.isValueCreated = true;
            }
        }

        return this.value;
    }
}
