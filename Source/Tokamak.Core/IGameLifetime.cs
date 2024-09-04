using System.Reflection.PortableExecutable;

namespace Tokamak.Core
{
    /// <summary>
    /// Service used to shutdown the GameHost
    /// </summary>
    public interface IGameLifetime
    {
        /// <summary>
        /// Checks to see if the game is currently running or shutting down.
        /// </summary>
        bool Running { get; }

        /// <summary>
        /// Request the game shutdown.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Adds an ITick component to the main thread tick update.
        /// </summary>
        /// <param name="tick">Item to add</param>
        void AddTick(ITick tick);

        /// <summary>
        /// Removes an ITick component from the main thread tick update.
        /// </summary>
        /// <param name="tick">Item to remove</param>
        /// <returns>True if the item was found, false if not.</returns>
        bool RemoveTick(ITick tick);
    }
}
