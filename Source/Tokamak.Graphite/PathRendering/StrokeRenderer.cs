using System.Collections.Generic;
using System.Diagnostics;
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

        public IEnumerable<Vector2> Render()
        {
            Debug.Assert(m_stroke.Points.Count >= 2, "Need at least two points for a stroke!");

            int segmentCount = m_stroke.Points.Count;
            segmentCount -= m_stroke.Closed ? 0 : 1;

            if (segmentCount < 1)
                return [];

            var points = new List<Vector2>(m_stroke.Points.Count * 2);
            var segments = new PathSegment[segmentCount];
            int i;

            for (i = 0; i < segmentCount; ++i)
            {
                Vector2 p1 = m_stroke.Points[i];
                Vector2 p2 = m_stroke.Points[(i + 1) % m_stroke.Points.Count];

                segments[i] = new PathSegment(p1, p2);
            }

            /*
             * Use the last segment's direction for miter computation if closed.
             * 
             * Otherwise use the first segment's direction to effectively get a right angle to the same line.
             */
            Vector2 lastDirection = m_stroke.Closed ?
                segments[segmentCount - 1].Direction :
                segments[0].Direction;

            /*
             * Compiler demands this be assigned to here, even though
             * it isn't possible for segmentCount to be less than 1.
             */
            Vector2 miter = Vector2.Zero;

            for (i = 0; i < segmentCount; ++i)
            {
                var current = segments[i];

                // Figure out the miter for the current point.

                miter = ComputeMiter(current.Direction, lastDirection);

                points.Add(current.Start + miter);
                points.Add(current.Start - miter);

                lastDirection = current.Direction;
            }

            if (m_stroke.Closed)
            {
                // Re-add first points
                points.Add(points[0]);
                points.Add(points[1]);
            }
            else
            {
                --i;
                points.Add(segments[i].End + miter);
                points.Add(segments[i].End - miter);
            }

            return points;
        }
    }
}
