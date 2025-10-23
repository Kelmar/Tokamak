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

        private IEnumerable<PathSegment> InitSegments()
        {
            var points = new Queue<Vector2>(m_stroke.Points);
            Vector2 p1 = points.Dequeue();

            foreach (var action in m_stroke.Actions)
            {
                switch (action)
                {
                case PathAction.Line:
                    {
                        Vector2 p2 = points.Dequeue();
                        yield return new PathSegment(p1, p2);
                        p1 = p2;
                    }
                    break;

                case PathAction.BezierQuadradic:
                    {
                        Vector2 last = p1;

                        Vector2 p2 = points.Dequeue();
                        Vector2 p3 = points.Dequeue();

                        for (int i = 1; i <= m_curveResolution; ++i)
                        {
                            float delta = i * m_curveStepping;
                            Vector2 next = Bezier.QuadSolve(p1, p2, p3, delta);

                            yield return new PathSegment(last, next);
                            last = next;
                        }

                        Debug.Assert(last == p3, "Do not reach desired point in CubicSolve!");
                        p1 = p3;
                    }
                    break;

                case PathAction.BezierCubic:
                    {
                        Vector2 last = p1;

                        Vector2 p2 = points.Dequeue();
                        Vector2 p3 = points.Dequeue();
                        Vector2 p4 = points.Dequeue();

                        for (int i = 1; i <= m_curveResolution; ++i)
                        {
                            float delta = i * m_curveStepping;
                            Vector2 next = Bezier.CubicSolve(p1, p2, p3, p4, delta);

                            yield return new PathSegment(last, next);
                            last = next;
                        }

                        Debug.Assert(last == p4, "Do not reach desired point in CubicSolve!");
                        p1 = p4;
                    }
                    break;


                default:
                    throw new NotImplementedException($"Unknown path action {action}");
                }
            }

            if (m_stroke.Closed)
                yield return new PathSegment(p1, m_stroke.Points[0]);

            Debug.Assert(points.Count == 0, "Not all points used for generating segments");
        }

        public IEnumerable<Vector2> Render()
        {
            Debug.Assert(m_stroke.Points.Count >= 2, "Need at least two points for a stroke");

            var segments = InitSegments().ToList();

            var points = new List<Vector2>(segments.Count * 2);

            /*
             * Use the last segment's direction for miter computation if closed.
             * 
             * Otherwise use the first segment's direction to effectively get a right angle to the same line.
             */
            Vector2 lastDirection = m_stroke.Closed ?
                segments.Last().Direction :
                segments.First().Direction;

            foreach (var segment in segments)
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
                var segment = segments.Last();

                Vector2 miter = ComputeMiter(segment.Direction, segment.Direction);

                points.Add(segments.Last().End + miter);
                points.Add(segments.Last().End - miter);
            }

            return points;
        }
    }
}
