using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Quill;

namespace Tokamak.Graphite
{
    internal class FontRenderer : IFontRenderer
    {
        public Vector2 m_lastPoint = Vector2.Zero;

        public FontRenderer()
        {
            Path.Winding = Winding.CounterClockwise;
        }

        public Path Path { get; } = new();

        public Matrix3x2 Transform
        {
            get => Matrix3x2.Identity;
            set { }
        }

        public void DrawLine(Vector2 v1, Vector2 v2)
        {
            if (m_lastPoint != v1)
                Path.MoveTo(v1);

            Path.LineTo(v2);

            m_lastPoint = v2;
        }

        public void DrawCubic(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
        {
            if (m_lastPoint != v1)
                Path.MoveTo(v1);

            Path.BezierCubicCurveTo(v2, v3, v4);

            m_lastPoint = v4;
        }

        public void DrawQuadradic(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            if (m_lastPoint != v1)
                Path.MoveTo(v1);

            Path.BezierQuadradicCurveTo(v2, v3);

            m_lastPoint = v3;
        }
    }
}
