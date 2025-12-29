using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace Tokamak.Graphite.PathRendering
{
    internal class FillRenderer
    {
        private readonly Stroke m_stroke;

        public FillRenderer(Stroke stroke)
        {
            m_stroke = stroke;
        }

        public IEnumerable<Vector2> Render()
        {
            Debug.Assert(m_stroke.Points.Count > 2, "Need at least three points for a fill.");

            return [];
        }
    }
}
