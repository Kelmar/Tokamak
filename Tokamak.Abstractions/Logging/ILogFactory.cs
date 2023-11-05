namespace Tokamak.Abstractions.Logging
{
    public interface ILogFactory
    {
        ILogger GetLogger(string name);

        ILogger<T> GetLogger<T>();
    }
}
