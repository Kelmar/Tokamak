using System.Threading;
using System.Threading.Tasks;

namespace Tokamak.Core
{
    public interface IBackgroundService
    {
        Task StartAsync(CancellationToken cancellationToken = default);

        Task StopAsync(CancellationToken cancellationToken = default);
    }
}
