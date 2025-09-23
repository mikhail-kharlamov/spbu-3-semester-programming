namespace Lazy;

/// <summary>
/// Interface for classes that implements lazy computation.
/// </summary>
/// <typeparam name="T">Type that function returns.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Method for get the function result value.
    /// </summary>
    /// <returns>Result of function computation.</returns>
    T Get();
}
