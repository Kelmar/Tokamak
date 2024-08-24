using System;
using System.Diagnostics;

using Tokamak.Core.Config;
using Tokamak.Core.Services;

namespace Tokamak.Core
{
    public class GameHostBuilder
    {
        private Func<IConfigReader> m_configFactory;

        private Func<IServiceLocator> m_serviceLocatorFactory;
        private Action<IServiceLocator> m_serviceConfig = null;

        private IConfigReader m_config;
        private IServiceLocator m_services;

        private IGameHost m_result = null;

        public GameHostBuilder()
        {
            ConfigFactory = () => new BasicConfigReader();
            ServiceLocatorFactory = () => new ServiceLocator();
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

        public Func<IServiceLocator> ServiceLocatorFactory
        {
            get => m_serviceLocatorFactory;
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                m_serviceLocatorFactory = value;
            }
        }

        public GameHostBuilder ConfigureServices(Action<IServiceLocator> serviceConfig)
        {
            if (serviceConfig == null)
                throw new ArgumentNullException(nameof(serviceConfig));

            m_serviceConfig = serviceConfig;
            return this;
        }

        private void InitConfig()
        {
            Debug.Assert(ConfigFactory != null);

            m_config = ConfigFactory();
        }

        private void InitServices()
        {
            Debug.Assert(ServiceLocatorFactory != null);

            m_services = ServiceLocatorFactory();
            m_services.Register(m_config);
            m_serviceConfig?.Invoke(m_services);
        }

        private IGameHost CreateHost()
        {
            return new Implementation.GameHost(m_services);
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
