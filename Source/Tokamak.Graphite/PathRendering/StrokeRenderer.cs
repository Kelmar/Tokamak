using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Graphite.PathRendering
{
    /*
     * To help understand what the math is doing here it, is best to think
     * of the line as being more like a vector with a start and a direction
     * pointing to the end.  When we consider this, the line then has a left
     * side and a right side which is based on it's direction:
     * 
     *         left
     * start --------> end
     *         right
     * 
     * It is important to note that this notion of left and right physically
     * changes on screen, depending on the relation of start to end.
     * 
     * For example:
     * 
     *        right
     * end <--------- start
     *        left
     * 
     * This can help us determine a few things.  For lines with a thickness,
     * we can consider the left side the "outside" of our path, and the right
     * the "inside" of our path, provided our path is drawn with a clockwise
     * winding.
     * 
     * This also helps us figure out which way a line is turning when we hit
     * a new point and it goes off into a new direction.
     * 
     * We do this by taking the cross product of the two vectors, or rather
     * we compute the Z component of a cross product of our 2D vectors as
     * if they are 3D vectors with their Z component set to zero, which
     * effectively will always return zero for the X and Y in the resulting
     * cross product.
     */

    internal class StrokeRenderer
    {
        private readonly Stroke m_stroke;

        private readonly int m_curveResolution;
        private readonly float m_curveStepping;

        private readonly float m_halfWidth;

        private readonly List<PathSegment> m_segments = new();

        /// <summary />
        /// <param name="stroke">The stroke to render.</param>
        /// <param name="curveResolution">The number of steps to compute for a curve.</param>
        /// <param name="lineWidth">The width of the pen line.</param>
        /// <remarks>
        /// Note that the more steps in a curve resolution we have, the smoother the resulting
        /// apparent curve approximation we'll get, but this comes at a cost to compute time.
        /// </remarks>
        public StrokeRenderer(Stroke stroke, int curveResolution, float lineWidth)
        {
            m_stroke = stroke;

            m_curveResolution = curveResolution;
            m_curveStepping = 1f / m_curveResolution;

            m_halfWidth = lineWidth / 2f;
        }

        private Vector2 ComputeMiter(in Vector2 direction, in Vector2 lastDirection)
        {
            Vector2 miter = ((direction + lastDirection) / 2).LineNormal();
            return miter * m_halfWidth / Vector2.Dot(miter, lastDirection.LineNormal());
        }

        private Vector2 ComputeStepped(in Vector2 first, bool ignoreFirst, Func<float, Vector2> callback)
        {
            // Less-than or equal is deliberate, we want to include "1"
            m_segments.EnsureCapacity(m_segments.Count + m_curveResolution + 1);

            Vector2 last = first;

            for (int i = 0; i <= m_curveResolution; ++i)
            {
                Vector2 next = callback(i * m_curveStepping);

                if (ignoreFirst)
                    ignoreFirst = false;
                else
                    m_segments.Add(new PathSegment(last, next));

                last = next;
            }

            return last;
        }

        private void InitSegments()
        {
            m_segments.Clear();

            var points = new Queue<Vector2>(m_stroke.Points);
            PathAction lastAction = PathAction.Move;
            Vector2 p1 = points.Dequeue();

            foreach (var action in m_stroke.Actions)
            {
                switch (action)
                {
                case PathAction.Line:
                    {
                        Vector2 p2 = points.Dequeue();
                        m_segments.Add(new PathSegment(p1, p2));
                        p1 = p2;
                    }
                    break;

                case PathAction.BezierQuadradic:
                    {
                        Vector2 p2 = points.Dequeue();
                        Vector2 p3 = points.Dequeue();

                        Vector2 last = ComputeStepped(
                            p1,
                            lastAction == PathAction.Move,
                            d => Bezier.QuadSolve(p1, p2, p3, d));

                        Debug.Assert(last == p3, "Do not reach desired point in CubicSolve!");
                        p1 = p3;
                    }
                    break;

                case PathAction.BezierCubic:
                    {
                        Vector2 p2 = points.Dequeue();
                        Vector2 p3 = points.Dequeue();
                        Vector2 p4 = points.Dequeue();

                        Vector2 last = ComputeStepped(
                            p1,
                            lastAction == PathAction.Move,
                            d => Bezier.CubicSolve(p1, p2, p3, p4, d));

                        Debug.Assert(last == p4, "Do not reach desired point in CubicSolve!");
                        p1 = p4;
                    }
                    break;

                case PathAction.Arc:
                    {
                        Vector2 center = points.Dequeue();
                        Vector2 radius = points.Dequeue();
                        Vector2 startEnd = points.Dequeue();

                        float start = startEnd[0];
                        float end = startEnd[1];

                        p1 = ComputeStepped(p1, lastAction == PathAction.Move, d =>
                        {
                            float angle = float.Lerp(start, end, d);
                            return new Vector2(center.X + MathF.Cos(angle) * radius.X, center.Y + MathF.Sin(angle) * radius.Y);
                        });
                    }
                    break;

                default:
                    throw new NotImplementedException($"Unknown path action {action}");
                }
            }

            if (m_stroke.Closed)
                m_segments.Add(new PathSegment(p1, m_stroke.Points[0]));

            Debug.Assert(points.Count == 0, "Not all points used for generating segments");
        }

        public IEnumerable<Vector2> Render()
        {
            Debug.Assert(m_stroke.Points.Count >= 2, "Need at least two points for a stroke");

            InitSegments();

            var points = new List<Vector2>(m_segments.Count * 2);

            /*
             * Use the last segment's direction for miter computation if closed.
             * 
             * Otherwise use the first segment's direction to effectively get a right angle to the same line.
             */
            Vector2 lastDirection = m_stroke.Closed ?
                m_segments.Last().Direction :
                m_segments.First().Direction;

            foreach (var segment in m_segments)
            {
                // Figure out the miter for the current point.

                Vector2 miter = ComputeMiter(segment.Direction, lastDirection);

                points.Add(segment.Start + miter);
                points.Add(segment.Start - miter);

                lastDirection = segment.Direction;
            }

            if (m_stroke.Closed)
            {
                // Re-add first points
                points.Add(points[0]);
                points.Add(points[1]);
            }
            else
            {
                var segment = m_segments.Last();

                Vector2 miter = ComputeMiter(segment.Direction, segment.Direction);

                points.Add(m_segments.Last().End + miter);
                points.Add(m_segments.Last().End - miter);
            }

            return points;
        }
    }
}
