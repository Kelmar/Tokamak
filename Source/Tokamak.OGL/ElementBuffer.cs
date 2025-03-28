using System;

using Silk.NET.OpenGL;

using Tokamak.Tritium.Buffers;

namespace Tokamak.OGL
{
    internal class ElementBuffer : IElementBuffer
    {
        private readonly uint m_ebo;
        private readonly OpenGLLayer m_apiLayer;

        private readonly BufferUsageARB m_usageHint;

        public ElementBuffer(OpenGLLayer apiLayer, BufferUsage usage)
        {
            m_apiLayer = apiLayer;

            m_usageHint = usage.ToGLUsage();

            m_ebo = m_apiLayer.GL.GenBuffer();
        }

        public void Dispose()
        {
            if (m_ebo != 0)
                m_apiLayer.GL.DeleteBuffer(m_ebo);
        }

        public void Activate()
        {
            m_apiLayer.GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, m_ebo);
        }

        public unsafe void Set(in ReadOnlySpan<uint> data)
        {
            if (data.Length == 0)
                return; // OpenGL doesn't like it if we send an empty list.

            Activate();

            m_apiLayer.GL.BufferData(BufferTargetARB.ElementArrayBuffer, data, m_usageHint);
        }
    }
}
