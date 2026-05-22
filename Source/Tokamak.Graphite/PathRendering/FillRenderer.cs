using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;

using Tokamak.Tritium.Geometry;
using Tokamak.Mathematics;

namespace Tokamak.Graphite.PathRendering
{
    internal class FillRenderer
    {
        private readonly List<Contour> m_contours;

        public FillRenderer(List<Contour> contours)
        {
            m_contours = contours;
        }

        public void Render(Winding winding, Canvas canvas, Pen pen)
        {
            Contour? largest = null;
            double largestArea = 0;

            var outside = new List<Contour>(m_contours.Count);
            var holes = new List<Contour>(m_contours.Count);

            foreach (var c in m_contours)
            {
                var area = Vector2.PolyArea(c.Points);

                if ((area > 0) && (winding != Winding.Clockwise))
                {
                    outside.Add(c);
                }
                else if ((area < 0) && (winding != Winding.CounterClockwise))
                {
                    if (-area > largestArea)
                    {
                        largestArea = -area;
                        largest = c;
                    }

                    holes.Add(c);
                }
            }

            if (outside.Count == 0 && (largest != null))
            {
                // Could not find an outside poly, use largest hole instead.
                holes.Remove(largest);
                outside.Add(largest);

                // Reverse the point order.
                largest.Points.Reverse();
            }

#if false
            /*
             * We use a naïve approach simulating a bunch of triangle fans.
             */

            Debug.Assert(m_contour.Points.Count > 2, "Need at least three points for a fill.");

            m_contour.BuildSegments(m_curveResolution);

            var points = new List<Vector2>(m_contour.Segments.Count * 3);
            float windingFactor = m_contour.Winding == Winding.Clockwise ? 1 : -1; // Invert for counter clockwise winding.
            PathSegment lastSegment = m_contour.Segments[0];
            Vector2 first = lastSegment.Start;
            float lastCross = 0;

            foreach (var segment in m_contour.Segments.Skip(1))
            {
                float cross = Vector2.Cross(lastSegment.Direction, segment.Direction) * windingFactor;

                if ((cross < 0) && (lastCross > 0))
                {
                    // Concave shape, break here and start a new shape.
                    if (points.Count > 0)
                    {
                        canvas.Draw(PrimitiveType.TriangleList, points, pen.Color);
                        points.Clear();
                    }

                    first = segment.Start;
                    lastCross = cross;

                    continue;
                }

                lastCross = cross;

                points.AddRange([first, segment.Start, segment.End]);

                lastSegment = segment;
            }

            if (points.Count > 0)
                canvas.Draw(PrimitiveType.TriangleList, points, pen.Color);
#endif
        }
    }
}
