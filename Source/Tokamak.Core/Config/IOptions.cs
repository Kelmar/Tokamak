namespace Tokamak.Core.Config
{
    public interface IOptions<T>
        where T : class, new()
    {
        T Value { get; }
    }
}
