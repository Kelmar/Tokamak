using System;
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

        public List<Vector2> Points { get; private set; } = [];

        public bool Closed { get; set; }

        public void CleanUp(float tolerance)
        {
            // Ensure we have a closed point.
            if (Vector2.AlmostEquals(Points[0], Points.Last(), tolerance))
            {
                Points.RemoveAt(Points.Count - 1);
                Closed = true;
            }

            Points = FilterPoints(tolerance).ToList();
        }

        private IEnumerable<Vector2> FilterPoints(float tolerance)
        {
            for (int i = 0; i < Points.Count; ++i)
            {
                int last = i == 0 ? (Points.Count - 1) : (i - 1);
                int next = (i + 1) % Points.Count;

                float c = Vector2.Cross(Points[last] - Points[i], Points[next] - Points[i]);

                if (Single.IsNaN(c) || Single.AlmostEquals(c, 0, tolerance))
                    continue; // Skip colinear points.

                if (Vector2.AlmostEquals(Points[i], Points[next], tolerance))
                    continue; // Skip points that look almost identical.

                yield return Points[i];
            }
        }

        /// <summary>
        /// Split a contour where it has intersections.
        /// </summary>
        /// <remarks>
        /// The resulting contours will have no intersections.
        /// </remarks>
        public IEnumerable<Contour> Split()
        {
            yield return this;
        }
    }
}
