namespace Tokamak.Core.Config
{
    public interface IConfigOptions<in T>
        where T : class
    {
        string Section { get; set; }
    }
}
