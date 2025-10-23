using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Graphite.PathRendering;
using Tokamak.Mathematics;

namespace Tokamak.Graphite
{
    public class Path
    {
        internal List<Stroke> m_strokes = new();

        private Stroke m_current = new();

        public Path()
        {
            m_strokes.Add(m_current);
        }

        /// <summary>
        /// Add in first move to point if needed.
        /// </summary>
        /// <remarks>
        /// Add a staring point at the zero vector if there
        /// are no other points yet added to the Points list.
        /// </remarks>
        private void AddFirstMove()
        {
            if (m_current.Points.Count > 0 && !m_current.Closed)
                return;

            if (m_current.Closed)
            {
                Stroke l = m_current;

                m_current = new();
                m_current.Points.Add(l.Points.Last());
                m_strokes.Add(m_current);
            }
            else
                m_current.Points.Add(Vector2.Zero);
        }

        /// <summary>
        /// Moves the drawing cursor to a new location.
        /// </summary>
        /// <param name="v">The new location of the drawing cursor.</param>
        public void MoveTo(in Vector2 v)
        {
            if (m_current.Points.Count > 1)
            {
                // Create a new stroke.
                m_current = new();
                m_strokes.Add(m_current);
            }

            if (m_current.Points.Count == 0)
            {
                m_current.Points.Add(v);
                m_current.Actions.Add(PathAction.Move);
            }
            else
                m_current.Points[0] = v;
        }

        /// <summary>
        /// Draw a line from the current cursor location to the new one.
        /// </summary>
        /// <param name="v">The location to draw to.</param>
        public void LineTo(in Vector2 v)
        {
            AddFirstMove();

            m_current.Points.Add(v);
            m_current.Actions.Add(PathAction.Line);
        }

        /// <summary>
        /// Draw a quadradic Bézier curve starting from the current cursor location.
        /// </summary>
        /// <param name="control">The control point of the curve.</param>
        /// <param name="end">The ending point of the curve.</param>
        public void BezierQuadradicCurveTo(in Vector2 control, in Vector2 end)
        {
            AddFirstMove();

            m_current.Points.AddRange([control, end]);
            m_current.Actions.Add(PathAction.BezierQuadradic);
        }

        /// <summary>
        /// Draw a cubic Bézier curve starting from the current cursor location.
        /// </summary>
        /// <param name="control1">The first control point of the curve.</param>
        /// <param name="control2">The second control point of the curve.</param>
        /// <param name="end">The ending point of the curve.</param>
        public void BezierCubicCurveTo(in Vector2 control1, in Vector2 control2, in Vector2 end)
        {
            AddFirstMove();

            m_current.Points.AddRange([control1, control2, end]);
            m_current.Actions.Add(PathAction.BezierCubic);
        }

        /// <summary>
        /// Add an elliptical arc to the the path.
        /// </summary>
        /// <param name="center">The center of the ellipse to arc through.</param>
        /// <param name="radius">The X and Y radius of the ellipse to draw.</param>
        /// <param name="start">The starting angle to draw at.</param>
        /// <param name="end">The angle to end drawing at.</param>
        public void ArcTo(in Vector2 center, in Vector2 radius, float start, float end)
        {
            AddFirstMove();

            m_current.Points.AddRange([center, radius, new Vector2(start, end)]);
            m_current.Actions.Add(PathAction.Arc);
        }

        public void Rectangle(in Vector2 topLeft, in Vector2 bottomRight, float roundEdges = 0)
        {
            if (MathX.AlmostEquals(roundEdges, 0))
            {
                MoveTo(topLeft);

                m_current.Points.AddRange([
                    new Vector2(bottomRight.X, topLeft.Y),
                    bottomRight,
                    new Vector2(topLeft.X, bottomRight.Y)
                ]);

                m_current.Actions.AddRange([PathAction.Line, PathAction.Line, PathAction.Line]);
            }

            Close();
        }

        public void Close()
        {
            // Only close if we have enough points to make a meaningful loop.
            if (m_current.Points.Count < 3)
                return;

            m_current.Closed = true;
        }
    }
}
