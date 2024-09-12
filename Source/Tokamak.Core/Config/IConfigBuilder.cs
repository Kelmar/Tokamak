using Tokamak.Abstractions.Config;

namespace Tokamak.Core.Config
{
    public interface IConfigBuilder
    {
        void AddProvider(IConfigProvider provider);

        IConfiguration Build();
    }
}
