using System;
using System.Diagnostics;

using Stashbox;

using Tokamak.Core.Config;

namespace Tokamak.Core
{
    public class GameHostBuilder
    {
        private Func<IConfigReader> m_configFactory;

        private Func<IStashboxContainer> m_containerFactory;
        private Action<IStashboxContainer> m_containerConfig = null;

        private IConfigReader m_config;
        private IStashboxContainer m_container;

        private IGameHost m_result = null;

        public GameHostBuilder()
        {
            ConfigFactory = () => new BasicConfigReader();
            ContainerFactory = () => new StashboxContainer();
        }

        internal void ConfigureArguments(string[] args)
        {
            if (args == null)
                return;
        }

        public Func<IConfigReader> ConfigFactory
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

        public GameHostBuilder ConfigureServices(Action<IStashboxContainer> serviceConfig)
        {
            if (serviceConfig == null)
                throw new ArgumentNullException(nameof(serviceConfig));

            m_containerConfig = serviceConfig;
            return this;
        }

        private void InitConfig()
        {
            Debug.Assert(ConfigFactory != null);

            m_config = ConfigFactory();
        }

        private void InitServices()
        {
            Debug.Assert(ContainerFactory != null);

            m_container = ContainerFactory();
            m_container.RegisterInstance(m_config);
            m_containerConfig?.Invoke(m_container);
        }

        private IGameHost CreateHost()
        {
            return new Implementation.GameHost(m_container);
        }

        public IGameHost Build()
        {
            if (m_result != null)
                return m_result;

            InitConfig();
            InitServices();

            m_result = CreateHost();
            return m_result;
        }
    }
}
