using Silk.NET.OpenGL;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Buffer;

namespace Tokamak.OGL
{
    internal class ElementBuffer : IElementBuffer
    {
        private readonly uint m_ebo;
        private readonly GLDevice m_parent;

        private readonly BufferUsageARB m_usageHint;

        public ElementBuffer(GLDevice device, BufferType type)
        {
            m_parent = device;

            m_usageHint = type.ToGLType();

            m_ebo = m_parent.GL.GenBuffer();
        }

        public void Dispose()
        {
            if (m_ebo != 0)
                m_parent.GL.DeleteBuffer(m_ebo);
        }

        public void Activate()
        {
            m_parent.GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, m_ebo);
        }

        public unsafe void Set(in ReadOnlySpan<uint> values)
        {
            Activate();

            uint size = (uint)(sizeof(uint) * values.Length);

            m_parent.GL.BufferData(BufferTargetARB.ElementArrayBuffer, size, values, m_usageHint);
        }
    }
}
