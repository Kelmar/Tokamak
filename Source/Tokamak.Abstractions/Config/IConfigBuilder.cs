namespace Tokamak.Config.Abstractions
{
    public interface IConfigBuilder
    {
        void AddProvider(IConfigProvider provider);

        IConfiguration Build();
    }
}
