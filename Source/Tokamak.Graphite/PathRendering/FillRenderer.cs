using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;

using Tokamak.Mathematics;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Graphite.PathRendering
{
    internal class FillRenderer
    {
        private readonly Stroke m_stroke;

        private readonly int m_curveResolution;
        private readonly float m_curveStepping;

        private readonly List<PathSegment> m_segments = new();

        public FillRenderer(Stroke stroke, int curveResolution)
        {
            m_stroke = stroke;
            m_curveResolution = curveResolution;
            m_curveStepping = 1f / m_curveResolution;
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
            Debug.Assert(m_stroke.Points.Count > 2, "Need at least three points for a fill.");

            InitSegments();

            foreach (var segment in m_segments)
            {
                yield return segment.Start;
                yield return segment.End;
            }
        }
    }
}
