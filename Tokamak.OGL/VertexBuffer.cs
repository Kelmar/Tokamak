using System.Collections.Generic;
using System.Linq;

using OpenTK.Graphics.OpenGL;

using Tokamak.Formats;

namespace Tokamak.OGL
{
    internal class VertexBuffer<T> : IVertexBuffer<T>
        where T : struct
    {
        private readonly int m_vba;
        private readonly int m_vbo;

        private readonly VectorFormat.Info m_layoutInfo;

        public VertexBuffer()
        {
            m_layoutInfo = VectorFormat.GetLayoutOf<T>();

            m_vba = GL.GenVertexArray();
            GL.BindVertexArray(m_vba);

            m_vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);
        }

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
        }

        private void Activate()
        {
            GL.BindVertexArray(m_vba);
            GL.BindBuffer(BufferTarget.ArrayBuffer, m_vbo);
        }

        public void Set(IEnumerable<T> data)
        {
            Activate();
            var verts = data.ToArray();

            GL.BufferData(BufferTarget.ArrayBuffer, verts.Count() * m_layoutInfo.Size, verts, BufferUsageHint.StreamDraw);

            foreach (var item in m_layoutInfo.Items)
            {
                GL.VertexAttribPointer(item.Index, item.Count, VertexAttribPointerType.Float, false, item.Stride, item.Offset);
                GL.EnableVertexAttribArray(item.Index);
            }
        }
    }
}
