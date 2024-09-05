using System;

using Tokamak.Core.Utilities;
using Tokamak.Mathematics;

namespace Tokamak.Tritium.APIs
{
    public interface IAPILayer : IDisposable
    {
        /// <summary>
        /// Called when the display area is resized.
        /// </summary>
        /// <remarks>
        /// This is usually for when running under a Window and the user adjusts the window size.
        /// </remarks>
        event SimpleEvent<Point> OnResize;

        /// <summary>
        /// Called to render to the view.
        /// </summary>
        event SimpleEvent<double> OnRender;

        /// <summary>
        /// Get the current size of the view port in pixels.
        /// </summary>
        Point ViewBounds { get; }

        /// <summary>
        /// Swap front/back buffers if buffering is enabled.
        /// </summary>
        void SwapBuffers();
    }
}
