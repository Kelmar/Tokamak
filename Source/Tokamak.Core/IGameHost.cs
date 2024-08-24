using System;
using System.Threading;
using System.Threading.Tasks;

using Stashbox;

namespace Tokamak.Core
{
    public interface IGameHost : IDisposable
    {
        IDependencyResolver Services { get; }

        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellation = default);
    }
}
