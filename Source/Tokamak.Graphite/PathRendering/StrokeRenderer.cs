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
        private readonly Contour m_contour;

        private readonly int m_curveResolution;
        private readonly float m_halfWidth;

        /// <summary />
        /// <param name="contour">The contour to render.</param>
        /// <param name="curveResolution">The number of steps to compute for a curve.</param>
        /// <param name="lineWidth">The width of the pen line.</param>
        /// <remarks>
        /// Note that the more steps in a curve resolution we have, the smoother the resulting
        /// apparent curve approximation we'll get, but this comes at a cost to compute time.
        /// </remarks>
        public StrokeRenderer(Contour contour, int curveResolution, float lineWidth)
        {
            m_contour = contour;

            m_curveResolution = curveResolution;

            m_halfWidth = lineWidth / 2f;
        }

        private Vector2 ComputeMiter(in Vector2 direction, in Vector2 lastDirection)
        {
            Vector2 miter = ((direction + lastDirection) / 2).LineNormal();
            return miter * m_halfWidth / Vector2.Dot(miter, lastDirection.LineNormal());
        }

        // Renders as a set of triangle strips.
        private IEnumerable<Vector2> InnerRender()
        {
            Debug.Assert(m_contour.Points.Count >= 2, "Need at least two points for a contour");

            m_contour.BuildSegments(m_curveResolution);

            var points = new Vector2[2];

            /*
             * Use the last segment's direction for miter computation if closed.
             * 
             * Otherwise use the first segment's direction to effectively get a right angle to the same line.
             */
            Vector2 lastDirection = m_contour.Closed ?
                m_contour.Segments.Last().Direction :
                m_contour.Segments[0].Direction;

            Vector2 miter = ComputeMiter(m_contour.Segments[0].Direction, lastDirection);

            points[0] = m_contour.Segments[0].Start + miter;
            points[1] = m_contour.Segments[0].Start - miter;

            yield return points[0];
            yield return points[1];

            foreach (var segment in m_contour.Segments.Skip(1))
            {
                miter = ComputeMiter(segment.Direction, lastDirection);

                yield return (segment.Start + miter);
                yield return (segment.Start - miter);

                lastDirection = segment.Direction;
            }

            if (m_contour.Closed)
            {
                // Repeat first points
                yield return points[0];
                yield return points[1];
            }
            else
            {
                var segment = m_contour.Segments.Last();

                miter = ComputeMiter(segment.Direction, segment.Direction);

                yield return (segment.End + miter);
                yield return (segment.End - miter);
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
