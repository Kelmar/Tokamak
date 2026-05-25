using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

using Tokamak.Graphite.PathRendering;

namespace Tokamak.Graphite
{
    /// <summary>
    /// Class for building up a list of drawing calls.
    /// </summary>
    public class Path
    {
        private readonly List<PathCommand> m_commands = [];

        public Path()
        {
            Winding = Winding.Clockwise;
        }

        /// <summary>
        /// Sets how the winding rules should be applied to this path.
        /// </summary>
        public Winding Winding
        {
            get;
            set
            {
                if (field == value)
                    return;

                field = value;
                m_commands.Add(new WindingCommand(value));
            }
        }

        /// <summary>
        /// Moves the drawing cursor to a new location.
        /// </summary>
        /// <param name="x">The X coordinate to move to.</param>
        /// <param name="y">The Y coordinate to move to.</param>
        public void MoveTo(float x, float y) => MoveTo(new Vector2(x, y));

        /// <summary>
        /// Moves the drawing cursor to a new location.
        /// </summary>
        /// <param name="v">The new location of the drawing cursor.</param>
        public void MoveTo(in Vector2 v)
        {
            //if (m_commands.Last() is MoveToCommand m)
            //    m.Point = v;
            //else

                m_commands.Add(new MoveToCommand(v));
        }

        /// <summary>
        /// Draw a line from the current cursor location to the new one.
        /// </summary>
        /// <param name="v">The location to draw to.</param>
        public void LineTo(in Vector2 v)
        {
            m_commands.Add(new LineToCommand(v));
        }

        /// <summary>
        /// Draw a line from the current cursor location to the new one.
        /// </summary>
        /// <param name="x">X coordinate to draw to.</param>
        /// <param name="y">Y coordinate to draw to.</param>
        public void LineTo(float x, float y) => LineTo(new Vector2(x, y));

        /// <summary>
        /// Draw a quadradic Bézier curve starting from the current cursor location.
        /// </summary>
        /// <param name="control">The control point of the curve.</param>
        /// <param name="end">The ending point of the curve.</param>
        public void BezierQuadradicCurveTo(in Vector2 control, in Vector2 end)
            => m_commands.Add(new QuadToCommand(control, end));

        /// <summary>
        /// Draw a cubic Bézier curve starting from the current cursor location.
        /// </summary>
        /// <param name="control1">The first control point of the curve.</param>
        /// <param name="control2">The second control point of the curve.</param>
        /// <param name="end">The ending point of the curve.</param>
        public void BezierCubicCurveTo(in Vector2 control1, in Vector2 control2, in Vector2 end)
            => m_commands.Add(new CubicToCommand(control1, control2, end));

        private void AddArc(in Vector2 center, float radius, float start, float end)
            => AddArc(center, new Vector2(radius, radius), start, end);

        /// <summary>
        /// Add a circular arc to the the path.
        /// </summary>
        /// <param name="center">The center of the circle to arc through.</param>
        /// <param name="radius">The radius of the circle to draw.</param>
        /// <param name="start">The starting angle to draw at.</param>
        /// <param name="end">The angle to end drawing at.</param>
        public void ArcTo(in Vector2 center, float radius, float start, float end)
            => ArcTo(center, new Vector2(radius, radius), start, end);

        /// <summary>
        /// Add an elliptical arc to the the path.
        /// </summary>
        /// <param name="center">The center of the ellipse to arc through.</param>
        /// <param name="radius">The X and Y radius of the ellipse to draw.</param>
        /// <param name="start">The starting angle to draw at.</param>
        /// <param name="end">The angle to end drawing at.</param>
        public void ArcTo(in Vector2 center, in Vector2 radius, float start, float end)
            => AddArc(center, radius, start, end);

        private void AddArc(in Vector2 center, in Vector2 radius, float start, float end)
            => m_commands.Add(new ArcToCommand(center, radius, start, end));

        /// <summary>
        /// Draws a rectangle at the supplied coordinates.
        /// </summary>
        /// <param name="topLeft">The top/left point of the rectangle.</param>
        /// <param name="bottomRight">The bottom/right point of the rectangle.</param>
        public void Rectangle(in Vector2 topLeft, in Vector2 bottomRight)
            => Rectangle(RectF.FromCoordinates(topLeft, bottomRight));

        /// <summary>
        /// Draws a rectangle at the supplied coordinates.
        /// </summary>
        /// <param name="rect">Rectangle to draw.</param>
        public void Rectangle(in RectF rect)
        {
            m_commands.AddRange(
                new MoveToCommand(rect.TopLeft),
                new LineToCommand(rect.TopRight),
                new LineToCommand(rect.BottomRight),
                new LineToCommand(rect.BottomLeft),
                new CloseCommand()
            );

            Close();
        }

        /// <summary>
        /// Draws a rounded rectangle.
        /// </summary>
        /// <remarks>
        /// If roundEdges is close enough to zero, then a regular rectangle will be drawn.
        /// </remarks>
        /// <param name="topLeft">Top left corner of the rectangle.</param>
        /// <param name="bottomRight">Bottom right corner for the rectangle.</param>
        /// <param name="roundEdges">Amount to round the corners by</param>
        public void RoundRect(in Vector2 topLeft, in Vector2 bottomRight, float roundEdges)
            => RoundRect(RectF.FromCoordinates(topLeft, bottomRight), roundEdges);

        /// <summary>
        /// Draws a rounded rectangle.
        /// </summary>
        /// <remarks>
        /// If roundEdges is close enough to zero, then a regular rectangle will be drawn.
        /// </remarks>
        /// <param name="rect">Rectable bounds to draw.</param>
        /// <param name="roundEdges">Amount to round the corners by</param>
        public void RoundRect(in RectF rect, float roundEdges)
        {
            if (MathX.AlmostEquals(roundEdges, 0))
                Rectangle(rect);
            else
            {
                MoveTo(new Vector2(rect.Right - roundEdges, rect.Top));

                var arcPoint = new Vector2(rect.Right - roundEdges, rect.Top + roundEdges);
                AddArc(arcPoint, roundEdges, MathF.Tau * 0.75f, MathF.Tau);

                LineTo(rect.Right, rect.Bottom - roundEdges);

                arcPoint = new Vector2(rect.Right - roundEdges, rect.Bottom - roundEdges);
                AddArc(arcPoint, roundEdges, 0, MathF.PI / 2);

                LineTo(rect.Left + roundEdges, rect.Bottom);

                arcPoint = new Vector2(rect.Left + roundEdges, rect.Bottom - roundEdges);
                AddArc(arcPoint, roundEdges, MathF.PI / 2, MathF.PI);

                LineTo(rect.Left, rect.Top + roundEdges);

                arcPoint = new Vector2(rect.Left + roundEdges, rect.Top + roundEdges);
                AddArc(arcPoint, roundEdges, MathF.PI, MathF.Tau * 0.75f);

                Close();
            }
        }

        public void Close() => m_commands.Add(new CloseCommand());

        private static void ComputeStepped(
            int resolution,
            float stepping,
            Contour current,
            Func<float, Vector2> callback)
        {
            current.Points.EnsureCapacity(current.Points.Count + resolution + 1);

            // Get first point and see if we need to skip it or not.
            Vector2 v = callback(0);

            if ((current.Points.Count == 0) || !Vector2.AlmostEquals(current.Points.Last(), v, Canvas.TOLERANCE))
                current.Points.Add(v); // Not already in list, add it.

            // Less-than or equal is deliberate, we want to include "1"
            for (int i = 1; i <= resolution; ++i)
                current.Points.Add(callback(i * stepping));
        }

        /// <summary>
        /// Flatten the path into a series of countours.
        /// </summary>
        /// <param name="resolution">Curve subdivision resoltuion.</param>
        /// <returns>An enumerable list of countours.</returns>
        internal IEnumerable<Contour> Flatten(int resolution)
        {
            float stepping = 1f / resolution;

            Contour current = new()
            {
                Winding = Winding
            };

            foreach (var command in m_commands)
            {
                var foo = command;

                switch (command)
                {
                case CloseCommand:
                    if (current.Points.Count > 2)
                    {
                        current.Closed = true;
                        current.CleanUp(Canvas.TOLERANCE);
                    }

                    if (current.Points.Count > 1)
                    {
                        yield return current;

                        current = new()
                        {
                            Winding = Winding
                        };
                    }
                    break;

                case MoveToCommand move:
                    if (current.Points.Count > 1)
                    {
                        current.CleanUp(Canvas.TOLERANCE);
                        yield return current;

                        current = new()
                        {
                            Winding = Winding
                        };
                    }

                    current.Points.Add(move.Point);
                    break;

                case LineToCommand line:
                    if (current.Points.Count == 0)
                        current.Points.Add(Vector2.Zero);

                    current.Points.Add(line.Point);
                    break;

                case QuadToCommand quad:
                    {
                        if (current.Points.Count == 0)
                            current.Points.Add(Vector2.Zero);

                        Vector2 p1 = current.Points.Last();

                        ComputeStepped(
                            resolution, stepping,
                            current,
                            d => Bezier.QuadSolve(p1, quad.Control, quad.End, d));
                    }
                    break;

                case CubicToCommand cubic:
                    {
                        if (current.Points.Count == 0)
                            current.Points.Add(Vector2.Zero);

                        Vector2 p1 = current.Points.Last();

                        ComputeStepped(
                            resolution, stepping,
                            current,
                            d => Bezier.CubicSolve(p1, cubic.Control1, cubic.Control2, cubic.End, d));
                    }
                    break;

                case ArcToCommand arc:
                    ComputeStepped(
                        resolution, stepping,
                        current,
                        d =>
                        {
                            float angle = float.Lerp(arc.StartAngle, arc.EndAngle, d);
                            var comp = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
                            return arc.Center + (comp * arc.Radius);
                        }
                    );
                    break;

                case WindingCommand winding:
                    current.Winding = winding.Winding;
                    break;

                default:
                    // Shouldn't happen.
                    throw new NotImplementedException($"BUG: Unknown path action {command}");
                }
            }

            if (current.Points.Count > 1)
            {
                current.CleanUp(Canvas.TOLERANCE);
                yield return current;
            }
        }
    }
}
