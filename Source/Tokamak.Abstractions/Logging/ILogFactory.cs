namespace Tokamak.Logging.Abstractions
{
    public interface ILogFactory
    {
        ILogger Create(string name = "");

        ILogger<T> Create<T>();
    }
}
