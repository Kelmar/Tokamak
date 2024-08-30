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
        private readonly IDependencyResolver m_scope;

        private List<IBackgroundService> m_services = new();

        public GameHost(GameHostBuilder builder)
        {
            m_container = builder.Container;

            // Allow things to resolve us, but don't dispose, we're managing the lifetime of the container itself!
            m_container.PutInstanceInScope<IGameHost>(this, withoutDisposalTracking: true);

            m_container.Validate();

            m_scope = m_container.BeginScope();

            Configuration = m_scope.Resolve<IConfiguration>();

            Log = m_scope.Resolve<ILogger>();
        }

        virtual protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scope.Dispose();
                m_container.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IDependencyResolver Services => m_scope;

        public IConfiguration Configuration { get; }

        protected ILogger Log { get; }

        abstract protected Task StartHostAsync(CancellationToken cancellationToken);

        abstract protected Task StopHostAsync(CancellationToken cancellationToken);

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            m_services.AddRange(m_scope.ResolveAll<IBackgroundService>());

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
