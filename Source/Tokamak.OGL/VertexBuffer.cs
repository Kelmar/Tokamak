using System;
using System.Diagnostics;

using Silk.NET.OpenGL;

using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;

namespace Tokamak.OGL
{
    internal class VertexBuffer<T> : IVertexBuffer<T>
        where T : unmanaged
    {
        private readonly GL m_gl;

        private readonly uint m_vbo;

        private readonly VectorFormat.Info m_layoutInfo;
        

        private readonly BufferUsageARB m_usageHint;

        public VertexBuffer(GL gl, BufferUsage usage)
        {
            m_gl = gl;

            m_layoutInfo = VectorFormat.GetLayoutOf<T>();

            m_usageHint = usage.ToGLUsage();

            m_vbo = m_gl.GenBuffer();
            m_gl.BindBuffer(BufferTargetARB.ArrayBuffer, m_vbo);

            IsEmpty = true;
        }

        public void Dispose()
        {
            if (m_vbo != 0)
                m_gl.DeleteBuffer(m_vbo);
        }

        public bool IsEmpty { get; private set; }

        public unsafe void Activate()
        {
            m_gl.BindBuffer(BufferTargetARB.ArrayBuffer, m_vbo);

            foreach (var item in m_layoutInfo.Items)
            {
                m_gl.VertexAttribPointer((uint)item.Index, item.Count, item.BaseType.ToGLType(), false, (uint)item.Stride, (void*)item.Offset);
                m_gl.EnableVertexAttribArray((uint)item.Index);
            }
        }

        public void Set(in ReadOnlySpan<T> data)
        {
            if (data.Length == 0)
                return; // OpenGL doesn't like it if we send an empty list.

            Activate();

            m_gl.BufferData(BufferTargetARB.ArrayBuffer, data, m_usageHint);

            IsEmpty = false;
        }
    }
}
