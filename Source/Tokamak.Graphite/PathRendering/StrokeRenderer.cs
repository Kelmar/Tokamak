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

        /// <summary>
        /// List of <seealso cref="PointInfo"/> objects.
        /// </summary>
        private readonly List<PointInfo> m_points = new();

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
        public StrokeRenderer(Stroke stroke, int curveResolution, float lineWidth = 1)
        {
            m_stroke = stroke;

            m_curveResolution = curveResolution;
            m_curveStepping = 1f / m_curveResolution;

            m_halfWidth = lineWidth / 2f;

            InitPoints();

            ComputeNormalsAndMiter();
        }

        #region Initialization

        /// <summary>
        /// Initialize the m_points list
        /// </summary>
        /// <remarks>
        /// This is a first pass initialization loop; we compute the point and it's
        /// basic direction here, which is then used to compute details about the
        /// point normals and miter information.
        /// </remarks>
        private void InitPoints()
        {
            var pointQueue = new Queue<Vector2>(m_stroke.Points);
            var actions = new Queue<PathAction>(m_stroke.Actions);

            Vector2 current = pointQueue.Dequeue();

            // TODO: Is it worth while to precompute number of entries in the m_points list?

            while (actions.TryDequeue(out PathAction action))
            {
                switch (action)
                {
                case PathAction.Line:
                    current = AddLine(current, pointQueue);
                    break;

                case PathAction.BezierQuadradic:
                    current = AddQuadradic(current, pointQueue);
                    break;

                case PathAction.BezierCubic:
                    current = AddCubic(current, pointQueue);
                    break;

                default:
                    Debug.Fail($"Unknown path action {action}");
                    break;
                }
            }
        }

        private void AddPointInfo(in Vector2 point, in Vector2 next)
        {
            m_points.Add(new PointInfo
            {
                Point = point,
                Direction = next - point
            });
        }

        private Vector2 AddLine(in Vector2 current, Queue<Vector2> points)
        {
            Debug.Assert(points.Count > 0, "Not enough points for AddLine()");

            Vector2 next = points.Dequeue();

            /*
             * TODO: Need to check if were joining two lines or if the previous/
             * next items are curves.  If they're curves we will likely want to 
             * compute how we join them different to make them smooth.
             */

            AddPointInfo(current, next);

            return next;
        }

        private Vector2 Stepped(in Vector2 start, Func<float, Vector2> compute)
        {
            /*
             * Floating point values can be really imprecise when it comes to addition.
             * 
             * Take for example the following:
             * float delta = 0;
             * 
             * for (int i = 0; i < 100; ++i)
             *      delta += 0.01f;
             * 
             * Console.WriteLine("d = {0}", d); // This will NOT print 1 as expected.
             * 
             * For this reason we compute the step as:
             * delta = step * (1 / m_curveResolution);
             */

            Vector2 loopCurrent = start;

            for (int step = 1; step <= m_curveResolution; ++step)
            {
                Vector2 next = compute(step * m_curveStepping);
                AddPointInfo(loopCurrent, next);
                loopCurrent = next;
            }

            return loopCurrent; // Should correctly equal the last point.
        }

        private Vector2 AddQuadradic(in Vector2 current, Queue<Vector2> points)
        {
            Debug.Assert(points.Count >= 2, "Not enough points for AddQuadradic()");

            Vector2 start = current;

            Vector2 control = points.Dequeue();
            Vector2 last = points.Dequeue();

            Vector2 computeLast = Stepped(current, step => Bezier.QuadSolve(start, control, last, step));

            Debug.Assert(computeLast == last, "AddQuadradic() did not reach last point");

            return last;
        }

        private Vector2 AddCubic(in Vector2 current, Queue<Vector2> points)
        {
            Debug.Assert(points.Count >= 3, "Not enough points for AddCubic()");

            Vector2 start = current;

            Vector2 control1 = points.Dequeue();
            Vector2 control2 = points.Dequeue();
            Vector2 last = points.Dequeue();

            Vector2 computeLast = Stepped(current, step => Bezier.CubicSolve(start, control1, control2, last, step));

            Debug.Assert(computeLast == last, "AddCubic() did not reach last point");

            return last;
        }

        /// <summary>
        /// Fills out additional information for the <seealso cref="PointInfo" /> objects.
        /// </summary>
        /// <remarks>
        /// Second pass initialization loop, this is dependent on the computed direction value
        /// in the first pass initialization.
        /// </remarks>
        private void ComputeNormalsAndMiter()
        {
            PointInfo last = m_points.Last();

            for (int i = 0; i < m_points.Count; ++i)
            {
                PointInfo current = m_points[i];

                Vector2 tangent = Vector2.Normalize(last.Direction + current.Direction);
                current.Normal = tangent.LineNormal();

                float scale = m_halfWidth / Vector2.Dot(current.Normal, last.Normal);

                current.Miter *= current.Normal * scale;

                last = current;
            }
        }

        #endregion Initialization

        public IEnumerable<Vector2> Render()
        {
            // A very basic implementation just map the points +/- miter for now.

            var rval = new List<Vector2>(m_points.Count * 2 + (m_stroke.Closed ? 1 : 0));

            rval.AddRange(m_points.SelectMany<PointInfo, Vector2>(p => [p.Point + p.Miter, p.Point - p.Miter]));

            if (m_stroke.Closed)
            {
                var first = m_points.First();
                rval.AddRange([first.Point + first.Miter, first.Point - first.Miter]);
            }

            return rval;
        }
    }
}
