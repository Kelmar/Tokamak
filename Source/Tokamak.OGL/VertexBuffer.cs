using System;

using Silk.NET.OpenGL;

using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;

namespace Tokamak.OGL
{
    internal class VertexBuffer<T> : IVertexBuffer<T>
        where T : unmanaged
    {
        private readonly uint m_vba;
        private readonly uint m_vbo;

        private readonly VectorFormat.Info m_layoutInfo;
        private readonly OpenGLLayer m_apiLayer;

        private readonly BufferUsageARB m_usageHint;

        public VertexBuffer(OpenGLLayer apiLayer, BufferUsage usage)
        {
            m_apiLayer = apiLayer;

            m_layoutInfo = VectorFormat.GetLayoutOf<T>();

            m_usageHint = usage.ToGLUsage();

            m_vba = m_apiLayer.GL.GenVertexArray();
            m_apiLayer.GL.BindVertexArray(m_vba);

            m_vbo = m_apiLayer.GL.GenBuffer();
            m_apiLayer.GL.BindBuffer(BufferTargetARB.ArrayBuffer, m_vbo);
        }

        public void Dispose()
        {
            if (m_vbo != 0)
            {
                m_apiLayer.GL.BindVertexArray(m_vba);

                m_apiLayer.GL.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
                m_apiLayer.GL.DeleteBuffer(m_vbo);
            }

            if (m_vba != 0)
                m_apiLayer.GL.DeleteVertexArray(m_vba);
        }

        public void Activate()
        {
            m_apiLayer.GL.BindVertexArray(m_vba);
            m_apiLayer.GL.BindBuffer(BufferTargetARB.ArrayBuffer, m_vbo);
        }

        public unsafe void Set(in ReadOnlySpan<T> data)
        {
            Activate();

            m_apiLayer.GL.BufferData(BufferTargetARB.ArrayBuffer, data, m_usageHint);

            foreach (var item in m_layoutInfo.Items)
            {
                m_apiLayer.GL.VertexAttribPointer((uint)item.Index, item.Count, item.BaseType.ToGLType(), false, (uint)item.Stride, (void*)item.Offset);
                m_apiLayer.GL.EnableVertexAttribArray((uint)item.Index);
            }
        }
    }
}
