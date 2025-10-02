namespace Tokamak.Config.Abstractions
{
    public interface IConfigOptions<in T>
        where T : class
    {
        string Section { get; set; }
    }
}
