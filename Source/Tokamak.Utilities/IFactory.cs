namespace Tokamak.Utilities
{
    /// <summary>
    /// Interface to a factory class.
    /// </summary>
    /// <typeparam name="T">The type the factory will create.</typeparam>
    public interface IFactory<T>
        where T : class
    {
        T Build();
    }
}
