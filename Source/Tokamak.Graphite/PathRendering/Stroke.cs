using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Graphite.PathRendering
{
    /// <summary>
    /// Holds details of a single stroke along a path.
    /// </summary>
    /// <remarks>
    /// Calls to the MoveTo() function begin a new stroke.
    /// </remarks>
    internal class Stroke
    {
        public List<Vector2> Points { get; } = new();

        public List<PathAction> Actions { get; } = new();

        public bool Closed { get; set; }
    }
}
