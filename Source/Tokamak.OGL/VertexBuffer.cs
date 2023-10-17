using System;
using System.Collections.Generic;
using System.Linq;

using Silk.NET.OpenGL;

using Tokamak.Buffer;
using Tokamak.Formats;

namespace Tokamak.OGL
{
    internal class VertexBuffer<T> : IVertexBuffer<T>
        where T : unmanaged
    {
        private readonly uint m_vba;
        private readonly uint m_vbo;

        private readonly VectorFormat.Info m_layoutInfo;
        private readonly GLPlatform m_parent;

        private readonly BufferUsageARB m_usageHint;

        public VertexBuffer(GLPlatform device, BufferType type)
        {
            m_parent = device;

            m_layoutInfo = VectorFormat.GetLayoutOf<T>();

            m_usageHint = type.ToGLType();

            m_vba = m_parent.GL.GenVertexArray();
            m_parent.GL.BindVertexArray(m_vba);

            m_vbo = m_parent.GL.GenBuffer();
            m_parent.GL.BindBuffer(BufferTargetARB.ArrayBuffer, m_vbo);
        }

        public void Dispose()
        {
            if (m_vbo != 0)
            {
                m_parent.GL.BindVertexArray(m_vba);

                m_parent.GL.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
                m_parent.GL.DeleteBuffer(m_vbo);
            }

            if (m_vba != 0)
                m_parent.GL.DeleteVertexArray(m_vba);
        }

        public void Activate()
        {
            m_parent.GL.BindVertexArray(m_vba);
            m_parent.GL.BindBuffer(BufferTargetARB.ArrayBuffer, m_vbo);
        }

        public unsafe void Set(IEnumerable<T> data)
        {
            Activate();
            var verts = data.ToArray();

            var vertSpan = new ReadOnlySpan<T>(verts);

            m_parent.GL.BufferData(BufferTargetARB.ArrayBuffer, vertSpan, m_usageHint);

            foreach (var item in m_layoutInfo.Items)
            {
                m_parent.GL.VertexAttribPointer((uint)item.Index, item.Count, item.BaseType.ToGLType(), false, (uint)item.Stride, (void*)item.Offset);
                m_parent.GL.EnableVertexAttribArray((uint)item.Index);
            }
        }
    }
}
