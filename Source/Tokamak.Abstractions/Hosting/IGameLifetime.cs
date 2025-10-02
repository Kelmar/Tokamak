namespace Tokamak.Hosting.Abstractions
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
        /// Run all tick events
        /// </summary>
        void Tick();

        /// <summary>
        /// Adds an ITick component to the main thread tick update.
        /// </summary>
        /// <param name="tick">Item to add</param>
        void AddTick(ITick tick, TickPriority priority = TickPriority.Normal);

        /// <summary>
        /// Removes an ITick component from the main thread tick update.
        /// </summary>
        /// <param name="tick">Item to remove</param>
        /// <returns>True if the item was found, false if not.</returns>
        bool RemoveTick(ITick tick);
    }
}
