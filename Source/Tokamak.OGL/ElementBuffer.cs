using System;

using Silk.NET.OpenGL;

using Tokamak.Tritium.Buffers;

namespace Tokamak.OGL
{
    internal class ElementBuffer : IElementBuffer
    {
        private readonly GL m_gl;

        private readonly uint m_ebo;

        private readonly BufferUsageARB m_usageHint;

        public ElementBuffer(GL gl, BufferUsage usage)
        {
            m_gl = gl;

            m_usageHint = usage.ToGLUsage();

            m_ebo = m_gl.GenBuffer();

            IsEmpty = true;
        }

        public void Dispose()
        {
            if (m_ebo != 0)
                m_gl.DeleteBuffer(m_ebo);
        }

        public bool IsEmpty { get; private set; }

        public void Activate()
        {
            m_gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, m_ebo);
        }

        public unsafe void Set(in ReadOnlySpan<uint> data)
        {
            if (data.Length == 0)
                return; // OpenGL doesn't like it if we send an empty list.

            Activate();

            m_gl.BufferData(BufferTargetARB.ElementArrayBuffer, data, m_usageHint);

            IsEmpty = false;
        }
    }
}
