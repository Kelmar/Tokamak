using Stashbox;

using System;
using System.Threading;
using System.Threading.Tasks;

using Tokamak.Core;

namespace TestBed
{
    public class GuiHost : IBackgroundService
    {
        private MainWindow m_window;

        public GuiHost(IDependencyResolver resolver)
        {
            m_window = resolver.Activate<MainWindow>();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                m_window.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() => m_window.Run());
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
