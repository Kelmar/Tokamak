using System;

using Tokamak.Mathematics;

namespace Tokamak.Tritium.APIs
{
    public interface IAPILayer : IDisposable
    {
        /// <summary>
        /// Get the current size of the view port in pixels.
        /// </summary>
        Point ViewBounds { get; }
    }
}
