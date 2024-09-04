﻿using System;

using Stashbox;

namespace Tokamak.Core
{
    /// <summary>
    /// Manages overall application lifecycle.
    /// </summary>
    public interface IGameHost : IDisposable
    {
        IDependencyResolver Services { get; }

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
