using System;
using System.Threading;
using System.Threading.Tasks;

using Tokamak.Core.Services;

namespace Tokamak.Core
{
    public interface IGameHost : IDisposable
    {
        IServiceLocator Services { get; }

        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellation = default);
    }
}
