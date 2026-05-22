using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Graphite.PathRendering
{
    internal class StrokeRenderer
    {
        private Contour m_contour;

        private readonly float m_halfWidth;

        /// <summary />
        /// <param name="contour">The contour to render.</param>
        /// <param name="curveResolution">The number of steps to compute for a curve.</param>
        /// <param name="lineWidth">The width of the pen line.</param>
        /// <remarks>
        /// Note that the more steps in a curve resolution we have, the smoother the resulting
        /// apparent curve approximation we'll get, but this comes at a cost to compute time.
        /// </remarks>
        public StrokeRenderer(Contour contour, float lineWidth)
        {
            m_contour = contour;

            m_halfWidth = lineWidth / 2f;
        }

        private Vector2 ComputeMiter(in Vector2 direction, in Vector2 lastDirection)
        {
            Vector2 miter = ((direction + lastDirection) / 2).LineNormal();
            return miter * m_halfWidth / Vector2.Dot(miter, lastDirection.LineNormal());
        }

        private static Vector2 Direction(Vector2 current, Vector2 previous)
        {
            Vector2 r = current - previous;

            float len = r.Length();

            if (len > 0)
                r /= len;

            return r;
        }

        // Renders as a set of triangle strips.
        private IEnumerable<Vector2> InnerRender()
        {
            Debug.Assert(m_contour.Points.Count >= 2, "Need at least two points for a contour");

            var points = new Vector2[2];

            Vector2 lastDirection, direction;
            Vector2 miter;

            if (m_contour.Closed)
            {
                // Use last point for direction
                lastDirection = Direction(m_contour.Points.Last(), m_contour.Points[0]);
            }
            else
            {
                // Use same direction as the first point.
                lastDirection = Direction(m_contour.Points[0], m_contour.Points[1]);
            }

            direction = Direction(m_contour.Points[0], m_contour.Points[1]);
            miter = ComputeMiter(direction, lastDirection);

            points[0] = m_contour.Points[0] + miter;
            points[1] = m_contour.Points[1] - miter;

            yield return points[0];
            yield return points[1];

            for (int i = 1; i < m_contour.Points.Count; ++i)
            {
                int next = i + 1;

                if (next >= m_contour.Points.Count)
                {
                    direction = m_contour.Closed ?
                        Direction(m_contour.Points[i], m_contour.Points[0]) :
                        lastDirection;
                }
                else
                    direction = Direction(m_contour.Points[i], m_contour.Points[next]);

                miter = ComputeMiter(direction, lastDirection);

                yield return m_contour.Points[i] + miter;
                yield return m_contour.Points[i] - miter;

                lastDirection = direction;
            }

            if (m_contour.Closed)
            {
                // Repeat first points.
                yield return points[0];
                yield return points[1];
            }
        }

        public IEnumerable<Vector2> Render()
        {
            var points = new Vector2[2];
            int cnt = 0;

            // Convert our triangle strip into a triangle list.
            foreach (var p in InnerRender())
            {
                if (cnt < 2)
                    points[cnt++] = p;
                else
                {
                    yield return points[0];
                    yield return points[1];
                    yield return p;

                    points[0] = points[1];
                    points[1] = p;
                }
            }
        }
    }
}
