using Tokamak.Core.Utilities;

using Tokamak.Mathematics;

namespace Tokamak.Tritium.APIs.NullRender
{
    /// <summary>
    /// Dummy layer that doesn't actually do anything.
    /// </summary>
    internal sealed class NullLayer : IAPILayer
    {
        // These events are not called from this layer
#pragma warning disable 00067
        public event SimpleEvent<Point> OnResize;
        public event SimpleEvent<double> OnRender;
#pragma warning restore 00067

        public NullLayer()
        {
        }

        public void Dispose()
        {
        }

        public Point ViewBounds { get; } = new Point(640, 480);

        public void SwapBuffers()
        {
        }
    }
}
