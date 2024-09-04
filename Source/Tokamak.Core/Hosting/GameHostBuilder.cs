using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Microsoft.Extensions.Configuration;

using Stashbox;

using Tokamak.Core.Config;

namespace Tokamak.Core.Hosting
{
    public class GameHostBuilder : IGameHostBuilder
    {
        private readonly List<Action<IConfigurationBuilder>> m_hostConfigBuilders = new();
        private readonly List<Action<IConfigurationBuilder>> m_appConfigBuilders = new();
        private readonly List<Action<IStashboxContainer>> m_serviceConfigs = new();

        private readonly Lazy<IConfiguration> m_hostConfig;
        private readonly Lazy<IConfigurationRoot> m_appConfig;

        private Func<IStashboxContainer> m_containerFactory;

        private IStashboxContainer m_container;

        private IGameHost m_result = null;

        public GameHostBuilder()
        {
            ContainerFactory = () => new StashboxContainer();

            m_hostConfig = new Lazy<IConfiguration>(BuildHostConfig);
            m_appConfig = new Lazy<IConfigurationRoot>(BuildAppConfig);
        }

        public IStashboxContainer Container => m_container;

        public IHostEnvironment HostEnvironment { get; private set; }

        public IConfigurationRoot Configuration => m_appConfig.Value;

        public Func<IStashboxContainer> ContainerFactory
        {
            get => m_containerFactory;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                m_containerFactory = value;
            }
        }

        public IGameHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configFn)
        {
            var fn = configFn ?? throw new ArgumentNullException(nameof(configFn));
            m_hostConfigBuilders.Add(fn);
            return this;
        }

        public IGameHostBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configFn)
        {
            var fn = configFn ?? throw new ArgumentNullException(nameof(configFn));
            m_appConfigBuilders.Add(fn);
            return this;
        }

        public IGameHostBuilder ConfigureServices(Action<IStashboxContainer> serviceConfig)
        {
            if (serviceConfig == null)
                throw new ArgumentNullException(nameof(serviceConfig));

            m_serviceConfigs.Add(serviceConfig);
            return this;
        }

        private IConfiguration BuildHostConfig()
        {
            var configBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection();

            foreach (var fn in m_hostConfigBuilders)
                fn(configBuilder);

            return configBuilder.Build();
        }

        private IConfigurationRoot BuildAppConfig()
        {
            var configBuilder = new ConfigurationBuilder()
                .AddConfiguration(m_hostConfig.Value, shouldDisposeConfiguration: true);

            foreach (var fn in m_appConfigBuilders)
                fn(configBuilder);

            return configBuilder.Build();
        }

        private void InitEnvironment()
        {
            var hostConfig = m_hostConfig.Value;

            Assembly entry = Assembly.GetEntryAssembly();

            string name = hostConfig["ApplicationName"] ??
                entry?.GetName().Name ??
                "Tokamak";

            HostEnvironment = new HostEnvironment
            {
                ApplicationName = name,
                EnvironmentName = hostConfig["EnvironmentName"] ?? "Debug"
            };
        }

        private void InitServices()
        {
            Debug.Assert(ContainerFactory != null);

            m_container = ContainerFactory();

            m_container.RegisterInstance(HostEnvironment);

            m_container.RegisterInstance<IConfiguration>(Configuration);

            // Initialize default configuration readers
            m_container.Register(typeof(IConfigOptions<>), typeof(ConfigOptions<>));
            m_container.Register(typeof(IOptions<>), typeof(DefaultOptions<>));

            foreach (var fn in m_serviceConfigs)
                fn(m_container);
        }

        public IGameHost Build()
        {
            if (m_result != null)
                return m_result;

            InitEnvironment();
            InitServices();

            m_result = new GameHost(this);
            return m_result;
        }
    }
}
