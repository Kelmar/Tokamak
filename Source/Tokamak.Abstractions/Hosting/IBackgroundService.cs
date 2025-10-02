using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tokamak.Hosting.Abstractions
{
    /// <summary>
    /// Interface to services that run on separate background tasks.
    /// </summary>
    public interface IBackgroundService : IDisposable
    {
        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
