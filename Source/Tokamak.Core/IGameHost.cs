using System;
using System.Threading;
using System.Threading.Tasks;

using Stashbox;

namespace Tokamak.Core
{
    /// <summary>
    /// Manages overall application lifecycle.
    /// </summary>
    /// <remarks>
    /// The GameHost manages the dependency resolver and setup and tear down
    /// of the various configured background services as well as the main
    /// application thread.
    /// 
    /// For the ClientGameHost this includes creation of the main UI Window or View.
    /// </remarks>
    public interface IGameHost : IDisposable
    {
        IDependencyResolver Services { get; }

        /// <summary>
        /// Called to initialize the game host and background services.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StartAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Called to stop the game host and background services.
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        Task StopAsync(CancellationToken cancellation = default);
    }
}
