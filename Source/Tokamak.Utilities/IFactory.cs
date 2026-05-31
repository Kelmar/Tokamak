namespace Tokamak.Utilities
{
    /// <summary>
    /// Interface to a factory class.
    /// </summary>
    /// <typeparam name="T">The type the factory will create.</typeparam>
    public interface IFactory<out T>
        where T : class
    {
        T Build();
    }

    public interface IFactory<T, out TResult>
        where TResult : class
    {
        TResult Build(T arg);
    }

    public interface IFactory<T1, T2, out TResult>
    {
        TResult Build(T1 arg1, T2 arg2);
    }

    public interface IFactory<T1, T2, T3, out TResult>
    {
        TResult Build(T1 arg1, T2 arg2, T3 arg3);
    }
}
