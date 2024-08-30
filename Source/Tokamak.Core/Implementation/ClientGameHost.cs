using System.Threading;
using System.Threading.Tasks;

using Stashbox;

using Tokamak.Core.Logging;

namespace Tokamak.Core.Implementation
{
    internal class ClientGameHost : GameHost
    {
        private ClientView m_view = null;

        public ClientGameHost(GameHostBuilder builder)
            : base(builder)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                m_view?.Dispose();
        }

        protected override Task StartHostAsync(CancellationToken cancellationToken)
        {
            Log.Info("Initializing client host.");

            Log.Debug("Initializing client view.");
            m_view = Services.Activate<ClientView>();

            return Task.CompletedTask;
        }

        protected override Task StopHostAsync(CancellationToken cancellationToken)
        {
            m_view.Run(cancellationToken);

            return Task.CompletedTask;
        }
    }
}
