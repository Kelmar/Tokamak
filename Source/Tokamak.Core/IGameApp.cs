using System;

namespace Tokamak.Core
{
    public interface IGameApp : IDisposable
    {
        /// <summary>
        /// Called when the application window is first loaded.
        /// </summary>
        void OnLoad();

        /// <summary>
        /// Application is being shutdown.
        /// </summary>
        /// <remarks>
        /// This method provides a way to do any any clean up that needs to happen
        /// before the Dispose() method is called.
        /// </remarks>
        void OnShutdown();

        /// <summary>
        /// Update simulation tick.
        /// </summary>
        /// <param name="timeDelta">Time between calls.</param>
        void OnUpdate(double timeDelta);

        /// <summary>
        /// Render tick
        /// </summary>
        /// <remarks>
        /// This method will not get called on ServerGameHost instances.
        /// </remarks>
        /// <param name="timeDelta">Time between calls.</param>
        void OnRender(double timeDelta);
    }
}
