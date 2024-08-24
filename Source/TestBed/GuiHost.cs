using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Tokamak.Core;

namespace TestBed
{
    public class GuiHost : IBackgroundService
    {
        private MainWindow m_window;

        public GuiHost()
        {
            m_window = new MainWindow();
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return Task.Run(() => m_window.Run());
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            m_window.Dispose();
            return Task.CompletedTask;
        }
    }
}
