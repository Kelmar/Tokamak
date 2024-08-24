using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Tokamak.Core.Services;

namespace Tokamak.Core.Implementation
{
    internal class GameHost : IGameHost
    {
        public GameHost(IServiceLocator services)
        {
            Services = services;
            Services.Register<IGameHost>(this);
        }

        public void Dispose()
        {
            Services.Dispose();
        }

        public IServiceLocator Services { get; }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            var services = Services.GetAll<IBackgroundService>();

            return Task.WhenAll(services.Select(s => s.StartAsync(cancellationToken)));
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            var services = Services.GetAll<IBackgroundService>();

            return Task.WhenAll(services.Select(s => s.StopAsync(cancellationToken)));
        }
    }
}
