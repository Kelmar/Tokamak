using System.Numerics;

using Silk.NET.OpenGL;

using Tokamak.Tritium.APIs;

using Tokamak.Buffer;

namespace Tokamak.OGL
{
    internal class GLCommandList : ICommandList
    {
        private readonly TextureObject m_whiteTexture;

        private Pipeline m_pipeline;

        public GLCommandList(GL gl, TextureObject whiteTexture)
        {
            GL = gl;
            m_whiteTexture = whiteTexture;
        }

        public void Dispose()
        {
        }

        public GL GL { get; }

        public IPipeline Pipeline { get; set; }

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
            GL.DrawArrays(m_pipeline.Primitive, vertexOffset, (uint)vertexCount);
        }

        public void DrawElements(int length)
        {
            int indices = 0;
            GL.DrawElements(m_pipeline.Primitive, (uint)length, DrawElementsType.UnsignedInt, ref indices);
        }

        public void Begin()
        {
        }

        public void End()
        {
        }
    }
}
