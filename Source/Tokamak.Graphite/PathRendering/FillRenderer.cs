using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

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

        public IEnumerable<Vector2> Render()
        {
            Debug.Assert(m_stroke.Points.Count > 2, "Need at least three points for a fill.");

            m_stroke.BuildSegments(m_curveResolution);

            // We use a naïve approach simulating a bunch of triangle fans.
            Vector2 first = m_stroke.Segments[0].Start;

            foreach (var segment in m_stroke.Segments.Skip(1))
            {
                yield return first;
                yield return segment.Start;
                yield return segment.End;
            }
        }
    }
}
