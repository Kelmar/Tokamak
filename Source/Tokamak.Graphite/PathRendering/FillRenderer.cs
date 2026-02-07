using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Graphite.PathRendering
{
    internal class FillRenderer
    {
        private readonly Stroke m_stroke;

        private readonly int m_curveResolution;

        public FillRenderer(Stroke stroke, int curveResolution)
        {
            m_stroke = stroke;
            m_curveResolution = curveResolution;
        }

        public void Render(Canvas canvas, Pen pen)
        {
            Debug.Assert(m_stroke.Points.Count > 2, "Need at least three points for a fill.");

            m_stroke.BuildSegments(m_curveResolution);

            var points = new List<Vector2>(m_stroke.Segments.Count * 3);

            /*
             * We use a naïve approach simulating a bunch of triangle fans.
             * 
             * Note that this system will break if the shape is concaved.
             */
            PathSegment lastSegment = m_stroke.Segments[0];
            Vector2 first = lastSegment.Start;
            float lastCross = 0;

            foreach (var segment in m_stroke.Segments.Skip(1))
            {
                float cross = Vector2.Cross(lastSegment.Direction, segment.Direction);

                if (((cross < 0) && (lastCross > 0)) 
                    //|| ((cross > 0) && (lastCross < 0))
                    )
                {
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
        }
    }
}
