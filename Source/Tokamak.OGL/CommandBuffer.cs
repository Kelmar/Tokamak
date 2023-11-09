using Silk.NET.OpenGL;

using Tokamak.Buffer;

using TokPrimType = Tokamak.PrimitiveType;

namespace Tokamak.OGL
{
    internal class CommandBuffer : ICommandBuffer
    {
        private readonly TextureObject m_whiteTexture;

        private Pipeline m_pipeline;

        public CommandBuffer(GL gl, TextureObject whiteTexture)
        {
            GL = gl;
            m_whiteTexture = whiteTexture;
        }

        public void Dispose()
        {
        }

        public GL GL { get; }

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

            if ((int)flags != 0)
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
            GL.DrawElements(m_pipeline.Primitive, (uint)length, DrawElementsType.UnsignedInt, 0);
        }

        public void Reset()
        {
        }

        public void BeginPass()
        {
        }

        public void EndPass()
        {
        }

        public void Begin()
        {
        }

        public void End()
        {
        }

        public void Flush()
        {
        }
    }
}
