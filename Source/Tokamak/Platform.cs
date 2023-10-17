using System;
using System.Collections.Generic;
using System.Numerics;

using Tokamak.Buffer;
using Tokamak.Formats;
using Tokamak.Mathematics;
using Tokamak.Services;

namespace Tokamak
{
    public abstract class Platform : IDisposable
    {
        private readonly Stack<Matrix4x4> m_worldMatrixStack = new Stack<Matrix4x4>();

        static Platform()
        {
            Services = new ServiceLocator();
        }

        protected Platform()
        {
        }

        public virtual void Dispose()
        {
        }

        // I know people call this an anti-pattern, it was this or a full on DI framework. >_<
        public static IServiceLocator Services { get; }

        public Matrix4x4 WorldMatrix { get; set; }

        public Matrix4x4 ProjectionMatrix { get; set; }

        public Matrix4x4 ViewMatrix { get; set; }

        virtual public Rect Viewport { get; set; }

        public abstract void SetRenderState(RenderState state);

        public IList<Monitor> Monitors { get; protected set; }

        public void PushWorldMatrix(in Matrix4x4 newMatrix)
        {
            m_worldMatrixStack.Push(WorldMatrix);
            WorldMatrix = newMatrix;
        }

        public void PopWorldMatrix()
        {
            WorldMatrix = m_worldMatrixStack.Pop();
        }

        public abstract void ClearBuffers(GlobalBuffer buffers);

        public abstract IVertexBuffer<T> GetVertexBuffer<T>(BufferType type)
            where T : unmanaged;

        public abstract ITextureObject GetTextureObject(PixelFormat format, Point size);

        public abstract IElementBuffer GetElementBuffer(BufferType type);

        public abstract void ClearBoundTexture();

        public abstract IShaderFactory GetShaderFactory();

        public abstract void DrawArrays(PrimitiveType primitive, int vertexOffset, int vertexCount);

        public abstract void DrawElements(PrimitiveType primitive, int length);
    }
}
