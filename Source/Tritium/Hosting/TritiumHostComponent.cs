using Stashbox;

using System;

using Tokamak.Core;
using Tokamak.Core.Config;
using Tokamak.Core.Logging;

using Tokamak.Tritium.APIs;

namespace Tokamak.Tritium.Hosting
{
    [LogName("Tritium")]
    internal sealed class TritiumHostComponent : IHostComponent
    {
        private readonly ILogger m_log;
        private readonly TritiumConfig m_config;
        private readonly IGameHost m_host;

        private IAPILayer m_apiLayer = null;

        public TritiumHostComponent(
            ILogger<TritiumHostComponent> log,
            IOptions<TritiumConfig> config,
            IGameHost host)
        {
            m_log = log;
            m_config = config.Value;
            m_host = host;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        private void InitAPI()
        {
            var loader = m_host.Services.Activate<APILoader>();

            var descriptor = loader.SelectAPI();
            m_apiLayer = descriptor.Create();
            m_host.Services.PutInstanceInScope(m_apiLayer);

            m_apiLayer.OnRender += m_host.App.OnRender;
        }

        public void Start()
        {
            m_log.Debug("Tridium starting.");

            InitAPI();
        }

        public void Stop()
        {
            m_log.Debug("Tridium stopping.");

            m_apiLayer.OnRender -= m_host.App.OnRender;
        }
    }
}
