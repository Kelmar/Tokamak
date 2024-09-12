﻿using System;

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

        public unsafe void Set(in ReadOnlySpan<uint> values)
        {
            Activate();

            uint size = (uint)(sizeof(uint) * values.Length);

            m_apiLayer.GL.BufferData(BufferTargetARB.ElementArrayBuffer, size, values, m_usageHint);
        }
    }
}
