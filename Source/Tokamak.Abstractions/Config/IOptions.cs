namespace Tokamak.Abstractions.Config
{
    public interface IOptions<out T>
        where T : class
    {
        T Value { get; }
    }
}
