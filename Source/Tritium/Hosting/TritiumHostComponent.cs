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
        private readonly IDependencyResolver m_resolver;

        private IAPILayer m_apiLayer = null;

        public TritiumHostComponent(
            ILogger<TritiumHostComponent> log,
            IOptions<TritiumConfig> config,
            IDependencyResolver resolver)
        {
            m_log = log;
            m_config = config.Value;
            m_resolver = resolver;
        }

        public void Dispose()
        {
            m_apiLayer?.Dispose();
            GC.SuppressFinalize(this);
        }

        private void InitAPI()
        {
            var loader = m_resolver.Activate<APILoader>();

            var descriptor = loader.SelectAPI();
            m_apiLayer = descriptor.Create();
        }

        public void Start()
        {
            m_log.Debug("Tridium starting.");

            InitAPI();
        }

        public void Stop()
        {
            m_log.Debug("Tridium stopping.");
        }
    }
}
