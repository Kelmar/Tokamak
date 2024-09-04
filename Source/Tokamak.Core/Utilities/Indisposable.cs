using System;

namespace Tokamak.Core.Utilities
{
    /// <summary>
    /// Provides an IDisposable that does nothing.
    /// </summary>
    /// <remarks>
    /// Use for when an function needs to return a dummy IDisposable without allocating extra memory.
    /// </remarks>
    public sealed class Indisposable : IDisposable
    {
        public static Indisposable Instance { get; } = new Indisposable();

        /// <summary>
        /// Prevent direct creation
        /// </summary>
        private Indisposable()
        {
        }

        /// <summary>
        /// Dummy method
        /// </summary>
        public void Dispose()
        {
        }
    }
}
