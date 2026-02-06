using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Graphite.PathRendering
{
    /// <summary>
    /// Holds details of a single stroke along a path.
    /// </summary>
    /// <remarks>
    /// Calls to the MoveTo() function begin a new stroke.
    /// </remarks>
    internal class Stroke
    {
        public List<Vector2> Points { get; } = [];

        public List<PathAction> Actions { get; } = [];

        public bool Closed { get; set; }

        public List<PathSegment> Segments { get; } = [];

        private Vector2 ComputeStepped(
            int resolution,
            float stepping,
            in Vector2 first,
            bool ignoreFirst,
            Func<float, Vector2> callback)
        {
            Segments.EnsureCapacity(Segments.Count + resolution + 1);

            Vector2 last = first;

            // Less-than or equal is deliberate, we want to include "1"
            for (int i = 0; i <= resolution; ++i)
            {
                Vector2 next = callback(i * stepping);

                if (ignoreFirst)
                    ignoreFirst = false;
                else
                    Segments.Add(new PathSegment(last, next));

                last = next;
            }

            return last;
        }

        public void BuildSegments(int resolution)
        {
            float stepping = 1f / resolution;

            Segments.Clear();

            var points = new Queue<Vector2>(Points);
            PathAction lastAction = PathAction.Move;
            Vector2 p1 = points.Dequeue();

            foreach (var action in Actions)
            {
                switch (action)
                {
                case PathAction.Line:
                    {
                        Vector2 p2 = points.Dequeue();
                        Segments.Add(new PathSegment(p1, p2));
                        p1 = p2;
                    }
                    break;

                case PathAction.BezierQuadradic:
                    {
                        Vector2 p2 = points.Dequeue();
                        Vector2 p3 = points.Dequeue();

                        Vector2 last = ComputeStepped(
                            resolution, stepping,
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
                            resolution, stepping,
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

                        p1 = ComputeStepped(
                            resolution, stepping,
                            p1, lastAction == PathAction.Move, d =>
                            {
                                float angle = float.Lerp(start, end, d);
                                return new Vector2(center.X + MathF.Cos(angle) * radius.X, center.Y + MathF.Sin(angle) * radius.Y);
                            }
                        );
                    }
                    break;

                default:
                    throw new NotImplementedException($"Unknown path action {action}");
                }
            }

            if (Closed)
                Segments.Add(new PathSegment(p1, Points[0]));

            Debug.Assert(points.Count == 0, "Not all points used for generating segments");
        }
    }
}
