using System;
using System.Collections.Generic;
using System.Numerics;

using Tokamak.Mathematics;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Graphite
{
    /// <summary>
    /// A 2D drawing surface.
    /// </summary>
    /// <remarks>
    /// A canvas simply builds up a list of 2D drawing commands and then sends 
    /// them along to the device in a series of buffered draw commands.
    /// 
    /// The canvas is designed to be reused between frame calls so that it does
    /// not allocate memory several times over and over again.
    /// </remarks>
    public class Canvas : ICanvas
    {
        private readonly Context m_context;

        private readonly List<Vector2> m_path = new();

        internal Canvas(Context context)
        {
            m_context = context;
        }

        public Matrix3x2 Transform { get; set; } = Matrix3x2.Identity;

        public Color StrokeColor { get; set; } = Color.Black;

        public float StrokeWidth { get; set; } = 1;

        public Color FillColor { get; set; } = Color.Black;

        #region Basic Path Manipulation

        public void BeginPath()
        {
            m_path.Clear();
        }

        public void MoveTo(float x, float y) => MoveTo(new Vector2(x, y));

        public void MoveTo(in Vector2 v)
        {
            m_path.Clear();
            m_path.Add(v);
        }

        public void LineTo(in Vector2 v)
        {
            m_path.Add(v);
        }

        public void ClosePath()
        {
            if (m_path.Count <= 1)
                return; // Only add a path if it has more than one point in it.

            // Add in closing point
            m_path.Add(m_path[0]);
        }

        #endregion Basic Path Manipulation

        #region Additional Path Manipulation



        #endregion Additional Path Manipulation

        public void Stroke()
        {
            if (m_path.Count < 2 || StrokeWidth <= 0 || StrokeColor.Alpha == 0)
                return; // Do nothing

            m_context.Draw(PrimitiveType.TriangleStrip, m_path, StrokeColor);
        }

        public void Fill()
        {
            if (m_path.Count < 3 || FillColor.Alpha == 0)
                return; // Do nothing

            var res = new List<Vector2>();

            // Simulating a triangle fan (navie aproach)
            Vector2 first = m_path[0];
            Vector2 prev = m_path[1];

            for (int i = 2; i < m_path.Count; ++i)
            {
                res.AddRange([first, prev, m_path[i]]);
                prev = m_path[i];
            }

            m_context.Draw(PrimitiveType.TriangleList, res, FillColor);
        }
    }
}
