namespace Tokamak.Config.Abstractions
{
    public interface IOptions<out T>
        where T : class
    {
        T Value { get; }
    }
}
