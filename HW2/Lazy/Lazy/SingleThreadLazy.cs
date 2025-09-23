namespace Lazy;

/// <summary>
/// Class for lazy computation in single-thread mode.
/// </summary>
/// <param name="supplier">Function object for computation.</param>
/// <typeparam name="T">Type that function returns.</typeparam>
public class SingleThreadLazy<T>(Func<T> supplier) : ILazy<T>
{
    private Func<T>? supplier = supplier;

    private T? value;

    private bool isValueCreated = false;

    /// <summary>
    /// Method for get the function result value.
    /// </summary>
    /// <returns>Result of function computation.</returns>
    public T Get()
    {
        if (!this.isValueCreated)
        {
            this.value = this.supplier();
            this.supplier = null;
            this.isValueCreated = true;
        }

        return this.value;
    }
}
