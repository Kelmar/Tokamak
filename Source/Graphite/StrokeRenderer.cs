using System;
using System.Collections.Generic;
using System.Numerics;

using Tokamak.Mathematics;

using Tokamak.Tritium.Buffers.Formats;

namespace Graphite
{
    internal class StrokeRenderer
    {
        private readonly Stroke m_stroke;

        /// <summary>
        /// Used to compute the total number of verts we need after applying the miter and bevels.
        /// </summary>
        private int m_bevelCount = 0;

        private float m_halfWidth = 0;

        public StrokeRenderer(Stroke stroke)
        {
            m_stroke = stroke;
            Render();
        }

        public List<VectorFormatPCT> Vectors { get; private set; }

        private void Flatten()
        {
            int index0 = m_stroke.Points.Count - 1;
            int count = m_stroke.Points.Count;
            int index1 = 0;

            if (m_stroke.Points[0].Position == m_stroke.Points[index0].Position)
            {
                m_stroke.Points.RemoveAt(index0);
                m_stroke.Closed = true;
                --index0;
                --count;
            }
            else if (!m_stroke.Closed && index0 >= 1)
            {
                // We need to calculate the direction differently for an end cap.

                StrokePoint p2 = m_stroke.Points[index0 - 1];
                StrokePoint p0 = m_stroke.Points[index0];

                p0.ComputeDirectionTo(p2);

                index0 = index1++;
                --count;
            }

            for (int i = 0; i < count; ++i, index0 = index1++)
            {
                StrokePoint p0 = m_stroke.Points[index0];
                StrokePoint p1 = m_stroke.Points[index1];

                p0.ComputeDirectionTo(p1);
            }
        }

        private void AddVector(Vector2 vector)
        {
            Vectors.Add(new VectorFormatPCT
            {
                Point = new Vector3(vector.X, vector.Y, 0),
                Color = (Vector4)m_stroke.Pen.Color,
                TexCoord = new Vector2(0, 0)
            });
        }

        private void CalculateJoins()
        {
            int index0 = m_stroke.Points.Count - 1;
            //int leftCount = 0;

            float inverseWidth = m_halfWidth > 0 ? 1 / m_halfWidth : 0;

            m_bevelCount = 0;

            for (int index1 = 0; index1 < m_stroke.Points.Count; index0 = index1++)
            {
                StrokePoint p0 = m_stroke.Points[index0];
                StrokePoint p1 = m_stroke.Points[index1];

                Vector2 dl0 = new Vector2(p0.Direction.Y, -p0.Direction.X);
                Vector2 dl1 = new Vector2(p1.Direction.Y, -p1.Direction.X);

                p1.MiterDirection = (dl0 + dl1) / 2;

                float dmr = p1.MiterDirection.LengthSquared();

                if (dmr > 0)
                {
                    float scale = MathF.Min(1 / dmr, 600);
                    p1.MiterDirection *= scale;
                }

                float cross = MathX.Cross(p1.Direction, p0.Direction);

                if (cross > 0)
                {
                    p1.Flags |= PointFlags.Left;
                    //++leftCount;
                }

                float limit = MathF.Max(MathF.Min(p0.Length, p1.Length) * inverseWidth, 1);

                if ((dmr * limit * limit) < 1)
                    p1.Flags |= PointFlags.InnerBevel;

                if (p1.Flags.HasFlag(PointFlags.Corner))
                {
                    if ((dmr * m_stroke.Pen.MiterLimit * m_stroke.Pen.MiterLimit) < 1)
                        p1.Flags |= PointFlags.OuterBevel;
                }

                if ((p1.Flags & (PointFlags.InnerBevel | PointFlags.OuterBevel)) != 0)
                    ++m_bevelCount;
            }

            //m_convex = leftCount == Points.Count;
        }

        private int ButtCapGetVectorCount()
        {
            return 4;
        }

        private void ButtCapStart(StrokePoint point, in Vector2 lineDirection)
        {
            Vector2 dl = new Vector2(point.Direction.Y, -point.Direction.X) * m_halfWidth;

            AddVector(point.Position + dl);
            AddVector(point.Position - dl);
        }

        private void ButtCapEnd(StrokePoint point, in Vector2 lineDirection)
        {
            Vector2 dl = new Vector2(point.Direction.Y, -point.Direction.X) * m_halfWidth;

            AddVector(point.Position - dl);
            AddVector(point.Position + dl);
        }

        private void AllocateVectors()
        {
            int cnt = m_stroke.Points.Count;

            // We're assuming bevel for now.
            cnt += m_bevelCount * 5; // 5? I count 3

            if (m_stroke.Closed)
                ++cnt;
            else
                cnt += ButtCapGetVectorCount();

            Vectors = new List<VectorFormatPCT>(cnt * 2);
        }

        private void ChooseBevel(bool bevel, StrokePoint p0, StrokePoint p1, float width, out Vector2 v0, out Vector2 v1)
        {
            if (!bevel)
            {
                v0 = new Vector2(p1.Position.X + p0.Direction.Y * width, p1.Position.Y - p0.Direction.X * width);
                v1 = new Vector2(p1.Position.X + p1.Direction.Y * width, p1.Position.Y - p1.Direction.X * width);
            }
            else
            {
                v0 = new Vector2(p1.Position.X + p1.MiterDirection.Y * width, p1.Position.Y + p1.MiterDirection.X * width);
                v1 = new Vector2(p1.Position.X + p1.MiterDirection.Y * width, p1.Position.Y + p1.MiterDirection.X * width);
            }
        }

        private void DrawBevel(StrokePoint p0, StrokePoint p1, float leftWidth, float rightWidth)
        {
            Vector2 r0, r1, l0, l1;

            // We seem to be calculating this a lot.
            Vector2 dl0 = new Vector2(p0.Direction.Y, -p0.Direction.X);
            Vector2 dl1 = new Vector2(p1.Direction.Y, -p1.Direction.X);

            Vector2 d = p1.MiterDirection * leftWidth;

            //AddVector(p1.Postion + d);
            //AddVector(p1.Postion - d);

            if (p1.Flags.HasFlag(PointFlags.Left))
            {
                // Convex turn
                ChooseBevel(p1.Flags.HasFlag(PointFlags.InnerBevel), p0, p1, leftWidth, out l0, out l1);

                var v2 = p1.Position + dl0 * -rightWidth;
                var v3 = p1.Position + dl1 * -rightWidth;

                //AddVector(p1.Position + d);
                //AddVector(v2);
                //AddVector(v3);
                if (p1.Flags.HasFlag(PointFlags.OuterBevel))
                {
                    AddVector(l0);
                    AddVector(v2);

                    AddVector(l1);
                    AddVector(v3);
                }

                AddVector(l1);
                AddVector(v3);
            }
            else
            {
                // Concave turn
                ChooseBevel(p1.Flags.HasFlag(PointFlags.InnerBevel), p0, p1, -rightWidth, out r0, out r1);

                var v2 = p1.Position + dl0 * leftWidth;
                var v3 = p1.Position + dl1 * leftWidth;

                AddVector(v2);
                AddVector(r0);

                //AddVector(p1.Position - d);
                //AddVector(v2);
                //AddVector(v3);

                if (p1.Flags.HasFlag(PointFlags.OuterBevel))
                {
                    AddVector(v2);
                    AddVector(r0);

                    AddVector(v3);
                    AddVector(r1);
                }

                AddVector(v3);
                AddVector(r1);
            }
        }

        private void Render()
        {
            m_halfWidth = m_stroke.Pen.Width / 2;

            Flatten();
            CalculateJoins();
            AllocateVectors();

            Vector2 lineDirection;

            int index0, index1, end, start;

            if (m_stroke.Closed)
            {
                // Closed path
                index0 = m_stroke.Points.Count - 1;
                index1 = 0;
                start = 0;
                end = m_stroke.Points.Count;
            }
            else
            {
                // End cap needed
                index0 = 0;
                index1 = 1;
                start = 1;
                end = m_stroke.Points.Count - 1;
            }

            if (!m_stroke.Closed)
            {
                // Add start end cap
                lineDirection = (m_stroke.Points[index1].Position - m_stroke.Points[index0].Position).Normalize();
                ButtCapStart(m_stroke.Points[index0], lineDirection);
            }

            for (int j = start; j < end; ++j, index0 = index1++)
            {
                StrokePoint p1 = m_stroke.Points[index1];

                if (m_stroke.Pen.LineJoin == LineJoin.Bevel ||
                    (p1.Flags & (PointFlags.InnerBevel | PointFlags.OuterBevel)) != 0)
                {
                    StrokePoint p0 = m_stroke.Points[index0];
                    DrawBevel(p0, p1, m_halfWidth, m_halfWidth);
                }
                else
                {
                    Vector2 d = p1.MiterDirection * m_halfWidth;

                    AddVector(p1.Position + d);
                    AddVector(p1.Position - d);
                }
            }

            if (!m_stroke.Closed)
            {
                // Add an end cap
                lineDirection = (m_stroke.Points[index1].Position - m_stroke.Points[index0].Position).Normalize();
                ButtCapEnd(m_stroke.Points[index1], lineDirection);
            }
            else
            {
                // Close the stroke
                VectorFormatPCT v0 = Vectors[0];
                VectorFormatPCT v1 = Vectors[1];

                Vectors.Add(v0);
                Vectors.Add(v1);
            }
        }
    }
}
