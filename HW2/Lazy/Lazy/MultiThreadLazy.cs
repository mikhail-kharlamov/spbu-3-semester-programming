namespace Lazy;

/// <summary>
/// ...
/// </summary>
/// <param name="supplier"></param>
/// <typeparam name="T"></typeparam>
public class MultiThreadLazy<T>(Func<T> supplier) : ILazy<T>
{
    private Func<T>? supplier = supplier;
    
    private T value;
    
    private bool isValueCreated = false;
    
    private readonly Lock locker = new Lock();
    
    /// <summary>
    /// ...
    /// </summary>
    /// <returns>...</returns>
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
