using System.Threading;
using System.Threading.Tasks;

using Stashbox;

using Tokamak.Core.Logging;

namespace Tokamak.Core.Implementation
{
    internal class ClientGameHost : GameHost
    {
        private ClientWindow m_window = null;

        public ClientGameHost(GameHostBuilder builder)
            : base(builder)
        {
            Log.Info("Client host created.");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                m_window?.Dispose();
        }

        protected override Task StartHostAsync(CancellationToken cancellationToken)
        {
            Log.Debug("Creating main window.");
            m_window = Services.Activate<ClientWindow>();

            return Task.CompletedTask;
        }

        protected override Task StopHostAsync(CancellationToken cancellationToken)
        {
            m_window.Run();
            return Task.CompletedTask;
        }
    }
}
