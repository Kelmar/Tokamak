using System;

using Stashbox;

using Tokamak.Core.Config;

namespace Tokamak.Core
{
    /// <summary>
    /// Interface for performing initial host setup.
    /// </summary>
    public interface IGameHostBuilder
    {
        IConfiguration Configuration { get; }

        IStashboxContainer Container { get; }

        IGameHostBuilder ConfigureHostConfiguration(Action<IConfigBuilder> configFn);

        IGameHostBuilder ConfigureAppConfiguration(Action<IConfigBuilder> configFn);

        IGameHostBuilder ConfigureServices(Action<IStashboxContainer> serviceConfig);

        IGameHost Build();
    }
}
