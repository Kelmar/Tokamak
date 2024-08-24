using System;

using Tokamak.Core.Services;

namespace Tokamak.Core
{
    public class GameAppBuilder
    {
        private Action<IServiceLocator> m_serviceConfig = null;

        public void ConfigureServices(Action<IServiceLocator> serviceConfig)
        {
            m_serviceConfig = serviceConfig;
        }

        public GameApplication Build()
        {
            GameApplication rval = new GameApplication();

            m_serviceConfig?.Invoke(rval.Services);

            return rval;
        }
    }
}
