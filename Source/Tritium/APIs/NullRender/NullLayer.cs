using Tokamak.Mathematics;

namespace Tokamak.Tritium.APIs.NullRender
{
    /// <summary>
    /// Dummy layer that doesn't actually do anything.
    /// </summary>
    internal sealed class NullLayer : IAPILayer
    {
        public void Dispose()
        {
        }

        public Point ViewBounds { get; } = new Point(640, 480);

        public void DoEvents()
        {
        }
    }
}
