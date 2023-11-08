using Silk.NET.OpenGL;

using Tokamak.Buffer;

using TokPrimType = Tokamak.PrimitiveType;

namespace Tokamak.OGL
{
    internal class CommandBuffer : ICommandBuffer
    {
        private readonly TextureObject m_whiteTexture;

        public CommandBuffer(GL gl, TextureObject whiteTexture)
        {
            GL = gl;
            m_whiteTexture = whiteTexture;
        }

        public void Dispose()
        {
        }

        public GL GL { get; }

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

        public void DrawArrays(TokPrimType primative, int vertexOffset, int vertexCount)
        {
            GL.DrawArrays(primative.ToGLPrimitive(), vertexOffset, (uint)vertexCount);
        }

        public void DrawElements(TokPrimType primitive, int length)
        {
            GL.DrawElements(primitive.ToGLPrimitive(), (uint)length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
