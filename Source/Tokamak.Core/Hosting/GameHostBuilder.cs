using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Stashbox;

using Tokamak.Config.Abstractions;

using Tokamak.Core.Config;

namespace Tokamak.Core.Hosting
{
    public class GameHostBuilder : IGameHostBuilder
    {
        private readonly List<Action<IConfigBuilder>> m_hostConfigBuilders = new();
        private readonly List<Action<IHostEnvironment, IConfigBuilder>> m_appConfigBuilders = new();
        private readonly List<Action<IStashboxContainer>> m_serviceConfigs = new();

        private readonly Lazy<IConfiguration> m_hostConfig;
        private readonly Lazy<IConfiguration> m_appConfig;

        private Func<IStashboxContainer> m_containerFactory;

        private IStashboxContainer? m_container = null;

        private IGameHost? m_result = null;

        public GameHostBuilder()
        {
            m_containerFactory = () => new StashboxContainer();

            m_hostConfig = new Lazy<IConfiguration>(BuildHostConfig);
            m_appConfig = new Lazy<IConfiguration>(BuildAppConfig);
        }

        public IStashboxContainer Container
        {
            get
            {
                Debug.Assert(m_container != null);
                return m_container;
            }
        }

        public IHostEnvironment? HostEnvironment { get; private set; }

        public IConfiguration Configuration => m_appConfig.Value;

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

        public IGameHostBuilder ConfigureHostConfiguration(Action<IConfigBuilder> configFn)
        {
            var fn = configFn ?? throw new ArgumentNullException(nameof(configFn));
            m_hostConfigBuilders.Add(fn);
            return this;
        }

        public IGameHostBuilder ConfigureAppConfiguration(Action<IHostEnvironment, IConfigBuilder> configFn)
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
            var configBuilder = new ConfigBuilder()
                .AddInMemoryConfig();

            foreach (var fn in m_hostConfigBuilders)
                fn(configBuilder);

            return configBuilder.Build();
        }

        private IConfiguration BuildAppConfig()
        {
            Debug.Assert(HostEnvironment != null);

            var configBuilder = new ConfigBuilder()
                .AddConfiguration(m_hostConfig.Value);

            foreach (var fn in m_appConfigBuilders)
                fn(HostEnvironment, configBuilder);

            return configBuilder.Build();
        }

        private string GetEnvironmentName(Assembly? asm)
        {
            if (asm == null)
                return "Release";

            var attr = asm.GetCustomAttribute<DebuggableAttribute>();

            if (attr != null)
                return "Debug";

            return "Release";
        }

        private void InitEnvironment()
        {
            var hostConfig = m_hostConfig.Value;

            Assembly? entry = Assembly.GetEntryAssembly();

            string name = hostConfig["ApplicationName"] ??
                entry?.GetName().Name ??
                "Tokamak";

            HostEnvironment = new HostEnvironment
            {
                ApplicationName = name,
                EnvironmentName = hostConfig["EnvironmentName"] ?? GetEnvironmentName(entry)
            };
        }

        private void InitServices()
        {
            Debug.Assert(ContainerFactory != null);
            Debug.Assert(HostEnvironment != null);

            m_container = ContainerFactory();

            m_container.RegisterInstance(HostEnvironment);

            m_container.RegisterInstance(Configuration);

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

            Debug.Assert(m_container != null);
            Debug.Assert(HostEnvironment != null);

            m_result = new GameHost(this);
            return m_result;
        }
    }
}
