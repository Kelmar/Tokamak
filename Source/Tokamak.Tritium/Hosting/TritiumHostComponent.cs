using System;

using Tokamak.Logging.Abstractions;
using Tokamak.Hosting.Abstractions;

using Tokamak.Tritium.APIs;
using System.Diagnostics;

namespace Tokamak.Tritium.Hosting
{
    [LogName("Tritium")]
    internal sealed class TritiumHostComponent : IHostComponent
    {
        private readonly ILogger m_log;
        private readonly IGameHost m_host;
        private readonly Func<IGraphicsLayer> m_layerFactory;

        private IGraphicsLayer? m_apiLayer = null;

        public TritiumHostComponent(
            ILogger<TritiumHostComponent> log,
            IGameHost host,
            Func<IGraphicsLayer> layerFactory)
        {
            m_log = log;
            m_host = host;
            m_layerFactory = layerFactory;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Start()
        {
            m_log.Debug("Tritium starting.");

            m_apiLayer = m_layerFactory();

            Debug.Assert(m_apiLayer != null, "No API Layer created!");

            m_apiLayer.OnRender += m_host.App.OnRender;
            m_apiLayer.OnLoad += m_host.App.OnLoad;
        }

        public void Stop()
        {
            m_log.Debug("Tritium stopping.");

            Debug.Assert(m_apiLayer != null, "No API Layer created!");

            m_apiLayer.OnLoad -= m_host.App.OnLoad;
            m_apiLayer.OnRender -= m_host.App.OnRender;
        }
    }
}
