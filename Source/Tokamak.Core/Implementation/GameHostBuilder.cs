using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Extensions.Configuration;

using Stashbox;

using Tokamak.Core.Config;

namespace Tokamak.Core.Implementation
{
    internal abstract class GameHostBuilder : IGameHostBuilder
    {
        private Func<IConfigurationRoot> m_configFactory;

        private Func<IStashboxContainer> m_containerFactory;
        private Action<IStashboxContainer> m_containerConfig = null;

        private Lazy<IConfigurationRoot> m_config;
        private IStashboxContainer m_container;

        private IGameHost m_result = null;

        public GameHostBuilder()
        {
            ConfigFactory = DefaultConfigure;
            ContainerFactory = () => new StashboxContainer();

            m_config = new Lazy<IConfigurationRoot>(() =>
            {
                Debug.Assert(ConfigFactory != null);
                return ConfigFactory();
            });
        }

        public IStashboxContainer Container => m_container;

        internal void ConfigureArguments(string[] args)
        {
            if (args == null)
                return;
        }

        private IConfigurationRoot DefaultConfigure()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            ;

            return builder.Build();
        }

        public IConfigurationRoot Configuration => m_config.Value;

        public Func<IConfigurationRoot> ConfigFactory
        {
            get => m_configFactory;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                m_configFactory = value;
            }
        }

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

        public IGameHostBuilder ConfigureServices(Action<IStashboxContainer> serviceConfig)
        {
            if (serviceConfig == null)
                throw new ArgumentNullException(nameof(serviceConfig));

            m_containerConfig = serviceConfig;
            return this;
        }

        private void InitServices()
        {
            Debug.Assert(ContainerFactory != null);

            m_container = ContainerFactory();
            m_container.RegisterInstance<IConfiguration>(m_config.Value);
            m_container.Register(typeof(IOptions<>), typeof(Options<>));

            m_containerConfig?.Invoke(m_container);
        }

        protected abstract IGameHost CreateHost();

        public IGameHost Build()
        {
            if (m_result != null)
                return m_result;

            InitServices();

            m_result = CreateHost();
            return m_result;
        }
    }
}
