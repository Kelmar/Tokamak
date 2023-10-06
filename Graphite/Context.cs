using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak;
using Tokamak.Mathematics;

namespace Graphite
{
    public class Context
    {
        private readonly IRenderer m_renderer;

        public Context(IRenderer renderer)
        {
            m_renderer = renderer;
        }

        public void SetViewport(int x, int y)
        {
            m_renderer.SetViewport(x, y);

            
        }

        private void FlattenPath(Path path)
        {
            int p0Idx = path.Points.Count - 1;
            int count = path.Points.Count;
            int p1Idx = 0;

            if (path.Points[0].Location == path.Points[p0Idx].Location)
            {
                path.Points.RemoveAt(p0Idx);
                path.Closed = true;
                --p0Idx;
                --count;
            }
            else if (!path.Closed && p0Idx >= 1)
            {
                // We need to calculate the direction differently for an end cap

                Point p2 = path.Points[p0Idx - 1];
                Point p0 = path.Points[p0Idx];

                p0.Direction = p2.Location - p0.Location;
                p0.Length = p0.Direction.Length();
                p0.Direction /= (p0.Length > 0) ? p0.Length : 1;

                p0Idx = p1Idx++;
                --count;
            }

            for (int i = 0; i < count; ++i, p0Idx = p1Idx++)
            {
                Point p0 = path.Points[p0Idx];
                Point p1 = path.Points[p1Idx];

                p0.Direction = p1.Location - p0.Location;
                p0.Length = p0.Direction.Length();
                p0.Direction /= (p0.Length > 0) ? p0.Length : 1;
            }
        }

        private void CalculateJoins(Path path, float width, float miterLimit)
        {
            int p0Idx = path.Points.Count - 1;
            int leftCnt = 0;

            float inverseWidth = width > 0 ? 1 / width : 0;

            path.BevelCount = 0;

            for (int p1Idx = 0; p1Idx < path.Points.Count; p0Idx = p1Idx++)
            {
                Point p0 = path.Points[p0Idx];
                Point p1 = path.Points[p1Idx];

                Vector2 dl0 = new Vector2(p0.Direction.Y, -p0.Direction.X);
                Vector2 dl1 = new Vector2(p1.Direction.Y, -p1.Direction.X);

                p1.MiterDirection = (dl0 + dl1) / 2;

                float dmr = p1.MiterDirection.LengthSquared();

                if (dmr > 0)
                {
                    float scale = MathF.Min(1 / dmr, 600);
                    p1.MiterDirection *= scale;
                }

                // Clear all other flags
                p1.Flags &= PointFlags.Corner;

                float cross = MathX.Cross(p1.Direction, p0.Direction);

                if (cross > 0)
                {
                    p1.Flags |= PointFlags.Left;
                    ++leftCnt;
                }

                float limit = MathF.Max(MathF.Min(p0.Length, p1.Length) * inverseWidth, 1);

                if ((dmr * limit * limit) < 1)
                    p1.Flags |= PointFlags.InnerBevel;

                if (p1.Flags.HasFlag(PointFlags.Corner))
                {
                    if ((dmr * miterLimit * miterLimit) < 1)
                    {
                        p1.Flags |= PointFlags.OuterBevel;
                    }
                }

                if ((p1.Flags & (PointFlags.InnerBevel | PointFlags.OuterBevel)) != 0)
                    ++path.BevelCount;
            }

            path.Convex = leftCnt == path.Points.Count;
        }

        private int GetVertexCount(Pen pen, Path path)
        {
            int cnt = path.Points.Count;

            // We're assuming a bevel for now.
            cnt += path.BevelCount * 5; // 5?  I count 3

            if (path.Closed)
                ++cnt;
            else
                cnt += ButtCapGetVectorCount(pen);

            return cnt * 2;
        }

        private int ButtCapGetVectorCount(Pen pen)
        {
            return 4;
        }

        private void ButtCapStart(List<Vector2> vectors, float width, Point point, in Vector2 lineDirection)
        {
            Vector2 dl = new Vector2(point.Direction.Y, -point.Direction.X) * width;

            vectors.Add(point.Location + dl);
            vectors.Add(point.Location - dl);

            m_renderer.DebugLine(point.Location + dl, point.Location - dl, Color.DarkRed);
        }

        private void ButtCapEnd(List<Vector2> vectors, float width, Point point, in Vector2 lineDirection)
        {
            Vector2 dl = new Vector2(point.Direction.Y, -point.Direction.X) * width;

            vectors.Add(point.Location - dl);
            vectors.Add(point.Location + dl);

            m_renderer.DebugLine(point.Location - dl, point.Location + dl, Color.DarkRed);
        }

        private void ChooseBevel(bool bevel, Point p0, Point p1, float width, out Vector2 v0, out Vector2 v1)
        {
            if (!bevel)
            {
                v0 = new Vector2(p1.Location.X + p0.Direction.Y * width, p1.Location.Y - p0.Direction.X * width);
                v1 = new Vector2(p1.Location.X + p1.Direction.Y * width, p1.Location.Y - p1.Direction.X * width);
            }
            else
            {
                v0 = new Vector2(p1.Location.X + p1.MiterDirection.Y * width, p1.Location.Y + p1.MiterDirection.X * width);
                v1 = new Vector2(p1.Location.X + p1.MiterDirection.Y * width, p1.Location.Y + p1.MiterDirection.X * width);
            }
        }

        private void DrawBevel(List<Vector2> vectors, Point p0, Point p1, float leftWidth, float rightWidth)
        {
            Vector2 r0, r1, l0, l1;

            // We seem to be calculating this a lot.
            Vector2 dl0 = new Vector2(p0.Direction.Y, -p0.Direction.X);
            Vector2 dl1 = new Vector2(p1.Direction.Y, -p1.Direction.X);

            Vector2 d = p1.MiterDirection * leftWidth;

            //vectors.Add(p1.Location + d);
            //vectors.Add(p1.Location - d);

            //m_renderer.DebugLine(p1.Location + d, p1.Location - d, Color.LiteGreen);

            if (p1.Flags.HasFlag(PointFlags.Left))
            {
                // Convex turn
                ChooseBevel(p1.Flags.HasFlag(PointFlags.InnerBevel), p0, p1, leftWidth, out l0, out l1);

                //if (l0 != l1)
                //    m_renderer.DebugLine(l0, l1, Color.Yellow);
                //else
                //    m_renderer.DebugPoint(l0, Color.Yellow);

                //m_renderer.DebugPoint(p1.Location + d, Color.Yellow);

                var v2 = p1.Location + dl0 * -rightWidth;
                var v3 = p1.Location + dl1 * -rightWidth;

                //m_renderer.DebugLine(v2, v3, Color.Magenta);

                //vectors.Add(p1.Location + d);
                //vectors.Add(v2);
                //vectors.Add(v3);
                if (p1.Flags.HasFlag(PointFlags.OuterBevel))
                {
                    vectors.Add(l0);
                    vectors.Add(v2);

                    vectors.Add(l1);
                    vectors.Add(v3);
                }

                vectors.Add(l1);
                vectors.Add(v3);
            }
            else
            {
                // Concave turn
                ChooseBevel(p1.Flags.HasFlag(PointFlags.InnerBevel), p0, p1, -rightWidth, out r0, out r1);

                //if (r0 != r1)
                //    m_renderer.DebugLine(r0, r1, Color.LiteCyan);
                //else
                //    m_renderer.DebugPoint(r0, Color.LiteCyan);

                //m_renderer.DebugPoint(p1.Location - d, Color.LiteCyan);

                var v2 = p1.Location + dl0 * leftWidth;
                var v3 = p1.Location + dl1 * leftWidth;

                //m_renderer.DebugLine(v2, v3, Color.LiteBlue);

                vectors.Add(v2);
                vectors.Add(r0);

                //vectors.Add(p1.Location - d);
                //vectors.Add(v2);
                //vectors.Add(v3);

                if (p1.Flags.HasFlag(PointFlags.OuterBevel))
                {
                    vectors.Add(v2);
                    vectors.Add(r0);

                    vectors.Add(v3);
                    vectors.Add(r1);
                }

                vectors.Add(v3);
                vectors.Add(r1);
            }
        }

        public void StrokePath(Pen pen, Path path)
        {
            float halfWidth = pen.Width / 2;

            FlattenPath(path);
            CalculateJoins(path, halfWidth, pen.MiterLimit);

            // Calculate vertex count:
            int vertCount = GetVertexCount(pen, path);

            var vectors = new List<Vector2>(vertCount);

            Vector2 lineDirection;

            int p0Idx, p1Idx, end, start;

            if (path.Closed)
            {
                // Closed path
                p0Idx = path.Points.Count - 1;
                p1Idx = 0;
                start = 0;
                end = path.Points.Count;
            }
            else
            {
                // End cap needed
                p0Idx = 0;
                p1Idx = 1;
                start = 1;
                end = path.Points.Count - 1;
            }

            if (!path.Closed)
            {
                // Add end cap
                lineDirection = (path.Points[p1Idx].Location - path.Points[p0Idx].Location).Normalize();
                ButtCapStart(vectors, halfWidth, path.Points[p0Idx], lineDirection);
            }

            for (int j = start; j < end; ++j, p0Idx = p1Idx++)
            {
                Point p1 = path.Points[p1Idx];

                if (pen.LineJoin == LineJoin.Bevel ||
                    (p1.Flags & (PointFlags.InnerBevel | PointFlags.OuterBevel)) != 0)
                {
                    Point p0 = path.Points[p0Idx];
                    DrawBevel(vectors, p0, p1, halfWidth, halfWidth);
                }
                else
                {
                    Vector2 d = p1.MiterDirection * halfWidth;

                    vectors.Add(p1.Location + d);
                    vectors.Add(p1.Location - d);

                    m_renderer.DebugLine(p1.Location + d, p1.Location - d, Color.LiteGreen);
                }
            }

            if (!path.Closed)
            {
                // Add end cap
                lineDirection = (path.Points[p1Idx].Location - path.Points[p0Idx].Location).Normalize();
                ButtCapEnd(vectors, halfWidth, path.Points[p1Idx], lineDirection);
            }
            else
            {
                // Close the path
                Vector2 v0 = vectors[0];
                Vector2 v1 = vectors[1];

                vectors.Add(v0);
                vectors.Add(v1);
            }

            m_renderer.Stroke(vectors, pen.Color);
            //m_renderer.DebugPath(path, Color.Grey);
        }

        public void Flush()
        {
            m_renderer.Flush();
        }
    }
}
