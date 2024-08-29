using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Stashbox;

using Tokamak.Core.Logging;

namespace Tokamak.Core.Implementation
{
    internal abstract class GameHost : IGameHost
    {
        private readonly IStashboxContainer m_container;

        private List<IBackgroundService> m_services = new();

        public GameHost(GameHostBuilder builder)
        {
            m_container = builder.Container;

            Services = m_container.BeginScope();

            Configuration = Services.Resolve<IConfiguration>();

            Log = Services.Resolve<ILogger>();

            // Allow things to resolve us, but don't dispose, we're managing the lifetime of the container itself!
            Services.PutInstanceInScope<IGameHost>(this, withoutDisposalTracking: true);
        }

        virtual protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Services.Dispose();
                m_container.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IDependencyResolver Services { get; }

        public IConfiguration Configuration { get; }

        protected ILogger Log { get; }

        abstract protected Task StartHostAsync(CancellationToken cancellationToken);

        abstract protected Task StopHostAsync(CancellationToken cancellationToken);

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            m_services.AddRange(Services.ResolveAll<IBackgroundService>());

            await Task.WhenAll(m_services.Select(s => s.StartAsync(cancellationToken)));

            await StartHostAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await StopHostAsync(cancellationToken);

            await Task.WhenAll(m_services.Select(s => s.StopAsync(cancellationToken)));
        }
    }
}
