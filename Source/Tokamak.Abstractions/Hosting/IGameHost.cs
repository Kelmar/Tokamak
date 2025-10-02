using System;

namespace Tokamak.Hosting.Abstractions
{
    /// <summary>
    /// Manages overall application lifecycle.
    /// </summary>
    public interface IGameHost : IDisposable
    {
        /// <summary>
        /// Returns the instance of the game application.
        /// </summary>
        IGameApp App { get; }

        /// <summary>
        /// Called to initialize the game host and background services.
        /// </summary>
        void Start();

        /// <summary>
        /// Process main thread event loop.
        /// </summary>
        void MainLoop();

        /// <summary>
        /// Called to stop the game host and background services.
        /// </summary>
        void Stop();
    }
}
