using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Graphite.PathRendering
{
    /// <summary>
    /// Holds details of a path contour.
    /// </summary>
    internal class Contour
    {
        public Winding Winding { get; set; }

        public List<Vector2> Points { get; } = [];

        public bool Closed { get; set; }

        public void CleanUp()
        {
            // Ensure we have a closed point.
            if (Vector2.AlmostEquals(Points[0], Points.Last(), Canvas.TOLERANCE))
            {
                Points.RemoveAt(Points.Count - 1);
                Closed = true;
            }

            // TODO: Look for contours that overlap themselves.  (E.g. figure 8)
        }
    }
}
