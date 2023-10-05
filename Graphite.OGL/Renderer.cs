using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using Numerics = System.Numerics;

namespace Graphite.OGL
{
    public class Renderer : IRenderer
    {
        private static readonly int VERTEX_SIZE = Marshal.SizeOf<Vertex>();

        private enum CallType
        {
            Debug = 0,
            DebugPoint = 1,
            Stroke = 10,
        }

        private class Call
        {
            public int VertexOffset { get; set; }

            public int VertexCount { get; set; }

            public CallType Type { get; set; }

            public Color Color { get; set; }
        }

        private readonly ShaderProgram m_shader;

        private readonly int m_vba;

        private readonly int m_vbo;

        private readonly List<Vertex> m_verts = new List<Vertex>(128);

        private readonly List<Call> m_calls = new List<Call>(128);

        private Matrix4 m_projection;

        public Renderer()
        {
            m_shader = new ShaderProgram();

            m_vba = GL.GenVertexArray();
            GL.BindVertexArray(m_vba);

            m_vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);

            SetViewport(640, 480);
        }

        public bool WireFrame { get; set; } = false;

        public bool Debug { get; set; } = false;

        public void Dispose()
        {
            if (m_vbo != 0)
            {
                GL.BindVertexArray(m_vba);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.DeleteBuffers(1, new int[] { m_vbo });
            }

            if (m_vba != 0)
                GL.DeleteVertexArray(m_vba);

            m_shader.Dispose();
        }

        public void SetViewport(int x, int y)
        {
            if (x <= 0) throw new ArgumentOutOfRangeException(nameof(x));
            if (y <= 0) throw new ArgumentOutOfRangeException(nameof(y));

            var mat = Numerics.Matrix4x4.CreateOrthographicOffCenter(0, x, y, 0, -1, 1);
            m_projection = mat.ToOpenTK();
        }

        public void Stroke(IEnumerable<Numerics.Vector2> vectors, in Color color)
        {
            var verts = vectors.Select(p => new Vertex { X = p.X, Y = p.Y });

            var call = new Call
            {
                VertexOffset = m_verts.Count,
                VertexCount = verts.Count(),
                Type = CallType.Stroke,
                Color = color
            };

            m_verts.AddRange(verts);
            m_calls.Add(call);
        }

        public void DebugPoint(in Numerics.Vector2 v, in Color color)
        {
            // Draw a 4x4 "square" for a point

            var verts = new List<Vertex>
            {
                new Vertex { X = v.X - 2, Y = v.Y - 2},
                new Vertex { X = v.X + 2, Y = v.Y - 2},
                new Vertex { X = v.X + 2, Y = v.Y + 2},
                new Vertex { X = v.X - 2, Y = v.Y + 2}
            };

            var call = new Call
            {
                VertexOffset = m_verts.Count,
                VertexCount = 4,
                Type = CallType.DebugPoint,
                Color = color
            };

            m_verts.AddRange(verts);
            m_calls.Add(call);
        }

        public void DebugLine(in Numerics.Vector2 v1, in Numerics.Vector2 v2, in Color color)
        {
            var verts = new List<Vertex>
            {
                new Vertex { X = v1.X, Y = v1.Y },
                new Vertex { X = v2.X, Y = v2.Y }
            };

            var call = new Call
            {
                VertexOffset = m_verts.Count,
                VertexCount = 2,
                Type = CallType.Debug,
                Color = color
            };

            m_verts.AddRange(verts);
            m_calls.Add(call);
        }

        public void DebugPath(Path p, in Color color)
        {
            var verts = p.Points.Select(p => new Vertex { X = p.Location.X, Y = p.Location.Y }).ToList();

            if (p.Closed)
                verts.Add(new Vertex { X = p.Points[0].Location.X, Y = p.Points[0].Location.Y });

            var call = new Call
            {
                VertexOffset = m_verts.Count,
                VertexCount = verts.Count(),
                Type = CallType.Debug,
                Color = color
            };

            m_verts.AddRange(verts);
            m_calls.Add(call);
        }

        public void Flush()
        {
            m_shader.Use();

            m_shader.SetMatrix4("projection", ref m_projection);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.BindVertexArray(m_vba);
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);

            var verts = m_verts.ToArray();
            GL.BufferData(BufferTarget.ArrayBuffer, verts.Count() * VERTEX_SIZE, verts, BufferUsageHint.StreamDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, VERTEX_SIZE, 0);
            //GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, VERTEX_SIZE, 2 * sizeof(float));
            GL.EnableVertexAttribArray(0);
            //GL.EnableVertexAttribArray(1);

            foreach (var c in m_calls)
            {
                switch (c.Type)
                {
                case CallType.Stroke:
                    StrokeCall(c);
                    break;
                }
            }

            // Always render debug calls last so they are on top.
            if (Debug)
            {
                foreach (var c in m_calls.Where(c => c.Type < CallType.Stroke))
                    DrawDebug(c);
            }

            m_verts.Clear();
            m_calls.Clear();
        }

        private void DrawDebug(Call call)
        {
            Vector4 clr = call.Color.ToOpenTK();
            m_shader.SetVector4("inColor", ref clr);

            PrimitiveType mode = call.Type switch
            {
                CallType.Debug => PrimitiveType.LineStrip,
                CallType.DebugPoint => PrimitiveType.TriangleStrip,
                _ => PrimitiveType.LineStrip
            };

            if (call.VertexCount == 1)
                mode = PrimitiveType.Points;

            GL.DrawArrays(mode, call.VertexOffset, call.VertexCount);
        }

        private void StrokeCall(Call call)
        {
            Vector4 clr = call.Color.ToOpenTK();
            m_shader.SetVector4("inColor", ref clr);

            PrimitiveType mode = WireFrame ? PrimitiveType.LineStrip : PrimitiveType.TriangleStrip;

            GL.DrawArrays(mode, call.VertexOffset, call.VertexCount);
        }
    }
}
