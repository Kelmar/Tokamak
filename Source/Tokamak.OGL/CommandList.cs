using System;
using System.Diagnostics;
using System.Numerics;

using Silk.NET.OpenGL;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Pipelines;
using Tokamak.Utilities;

namespace Tokamak.OGL
{
    internal class CommandList : ICommandList
    {
        private readonly TextureObject m_whiteTexture;

        private Pipeline? m_pipeline;

        public CommandList(GL gl, TextureObject whiteTexture)
        {
            GL = gl;
            m_whiteTexture = whiteTexture;
        }

        public void Dispose()
        {
        }

        public GL GL { get; }

        public Vector4 ClearColor { get; set; }

        internal void MakeActive(Pipeline pipeline)
        {
            m_pipeline = pipeline;
        }

        public void ClearBuffers(GlobalBuffer buffers)
        {
            ClearBufferMask flags = 0;

            flags |= buffers.HasFlag(GlobalBuffer.ColorBuffer) ? ClearBufferMask.ColorBufferBit : 0;
            flags |= buffers.HasFlag(GlobalBuffer.DepthBuffer) ? ClearBufferMask.DepthBufferBit : 0;
            flags |= buffers.HasFlag(GlobalBuffer.StencilBuffer) ? ClearBufferMask.StencilBufferBit : 0;

            if (flags != 0)
                GL.Clear(flags);
        }

        public void ClearBoundTexture()
        {
            // Reset to our 1x1 white texture to keep samplers in shaders happy.
            m_whiteTexture.Activate();
        }

        public void DrawArrays(int vertexOffset, int vertexCount)
        {
            Debug.Assert(m_pipeline != null, "No pipeline selected!");

            GL.DrawArrays(m_pipeline.Primitive, vertexOffset, (uint)vertexCount);
        }

        public unsafe void DrawElements(int length)
        {
            Debug.Assert(m_pipeline != null, "No pipeline selected!");

            GL.DrawElements(m_pipeline.Primitive, (uint)length, DrawElementsType.UnsignedInt, null);
        }

        public IDisposable BeginScope() => Indisposable.Instance;
    }
}
