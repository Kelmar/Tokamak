using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Stashbox;

namespace Tokamak.Core.Implementation
{
    internal sealed class GameHost : IGameHost
    {
        private IStashboxContainer m_container;

        public GameHost(IStashboxContainer container)
        {
            m_container = container;

            Services = m_container.BeginScope();

            // Allow things to resolve us, but don't dispose, we're managing the lifetime of the container itself!
            Services.PutInstanceInScope<IGameHost>(this, withoutDisposalTracking: true);
        }

        public void Dispose()
        {
            Services.Dispose();
            m_container.Dispose();
        }

        public IDependencyResolver Services { get; }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            var services = Services.ResolveAll<IBackgroundService>();

            return Task.WhenAll(services.Select(s => s.StartAsync(cancellationToken)));
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            var services = Services.ResolveAll<IBackgroundService>();

            return Task.WhenAll(services.Select(s => s.StopAsync(cancellationToken)));
        }
    }
}
