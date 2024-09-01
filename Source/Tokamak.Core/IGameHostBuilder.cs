using Microsoft.Extensions.Configuration;

using Stashbox;

using System;

namespace Tokamak.Core
{
    /// <summary>
    /// Interface for performing initial host setup.
    /// </summary>
    public interface IGameHostBuilder
    {
        IConfigurationRoot Configuration { get; }

        Func<IStashboxContainer> ContainerFactory { get; set; }

        IGameHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configFn);

        IGameHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configFn);

        IGameHostBuilder ConfigureServices(Action<IStashboxContainer> serviceConfig);

        IGameHost Build();
    }
}
