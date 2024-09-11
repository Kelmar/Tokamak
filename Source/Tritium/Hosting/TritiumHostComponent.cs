using System;

using Tokamak.Abstractions.Config;
using Tokamak.Abstractions.Logging;

using Tokamak.Core;

using Tokamak.Tritium.APIs;

namespace Tokamak.Tritium.Hosting
{
    [LogName("Tritium")]
    internal sealed class TritiumHostComponent : IHostComponent
    {
        private readonly ILogger m_log;
        private readonly TritiumConfig m_config;
        private readonly IGameHost m_host;
        private readonly Func<IAPILayer> m_layerFactory;

        private IAPILayer m_apiLayer = null;

        public TritiumHostComponent(
            ILogger<TritiumHostComponent> log,
            IOptions<TritiumConfig> config,
            IGameHost host,
            Func<IAPILayer> layerFactory)
        {
            m_log = log;
            m_config = config.Value;
            m_host = host;
            m_layerFactory = layerFactory;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Start()
        {
            m_log.Debug("Tridium starting.");

            m_apiLayer = m_layerFactory();
            m_apiLayer.OnRender += m_host.App.OnRender;
            m_apiLayer.OnLoad += m_host.App.OnLoad;
        }

        public void Stop()
        {
            m_log.Debug("Tridium stopping.");

            m_apiLayer.OnLoad -= m_host.App.OnLoad;
            m_apiLayer.OnRender -= m_host.App.OnRender;
        }
    }
}
