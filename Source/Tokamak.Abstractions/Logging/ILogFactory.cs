namespace Tokamak.Abstractions.Logging
{
    public interface ILogFactory
    {
        ILogger Create(string name = "");

        ILogger<T> Create<T>();
    }
}
