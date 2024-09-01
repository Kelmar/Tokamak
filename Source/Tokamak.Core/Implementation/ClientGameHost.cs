using System.Threading;
using System.Threading.Tasks;

using Stashbox;

using Tokamak.Core.Drivers;
using Tokamak.Core.Logging;

namespace Tokamak.Core.Implementation
{
    internal class ClientGameHost : GameHost
    {
        private readonly IDriverLoader m_drivers = null;

        private ClientView m_view = null;

        public ClientGameHost(GameHostBuilder builder)
            : base(builder)
        {
            m_drivers = Services.ResolveOrDefault<IDriverLoader>();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                m_drivers?.Unload();
                m_view?.Dispose();
            }
        }

        protected override Task StartHostAsync(CancellationToken cancellationToken)
        {
            Log.Info("Initializing client host.");

            m_drivers?.Preload();

            Log.Debug("Initializing client view.");

            m_view = Services.Activate<ClientView>();

            Services.PutInstanceInScope<IOGLContextProvider>(m_view, withoutDisposalTracking: true);

            m_drivers?.Load();

            return Task.CompletedTask;
        }

        protected override Task StopHostAsync(CancellationToken cancellationToken)
        {
            m_view.Run(cancellationToken);

            return Task.CompletedTask;
        }
    }
}
