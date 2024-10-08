﻿using System;

using Stashbox;

using Tokamak.Abstractions.Config;

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

        IGameHostBuilder ConfigureAppConfiguration(Action<IHostEnvironment, IConfigBuilder> configFn);

        IGameHostBuilder ConfigureServices(Action<IStashboxContainer> serviceConfig);

        IGameHost Build();
    }
}
