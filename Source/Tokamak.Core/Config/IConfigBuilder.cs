using System.Collections.Generic;

namespace Tokamak.Core.Config
{
    public interface IConfigBuilder
    {
        void AddProvider(IConfigProvider provider);

        IConfiguration Build();
    }
}
