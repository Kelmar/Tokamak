using Microsoft.Extensions.Configuration;

using Stashbox;

using System;

namespace Tokamak.Core
{
    public interface IGameHostBuilder
    {
        IConfigurationRoot Configuration { get; }

        Func<IConfigurationRoot> ConfigFactory { get; set; }

        Func<IStashboxContainer> ContainerFactory { get; set; }

        IGameHostBuilder ConfigureServices(Action<IStashboxContainer> serviceConfig);

        IGameHost Build();
    }
}
