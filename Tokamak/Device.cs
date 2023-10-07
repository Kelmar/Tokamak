using System;
using System.Collections.Generic;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak
{
    public abstract class Device : IDisposable
    {
        private readonly Stack<Matrix4x4> m_worldMatrixStack = new Stack<Matrix4x4>();

        protected Device()
        {
        }

        public virtual void Dispose()
        {
        }

        public Matrix4x4 WorldMatrix { get; set; }

        public Matrix4x4 ProjectionMatrix { get; set; }

        public Matrix4x4 ViewMatrix { get; set; }

        virtual public Rect Viewport { get; set; }

        public void PushWorldMatrix(in Matrix4x4 newMatrix)
        {
            m_worldMatrixStack.Push(WorldMatrix);
            WorldMatrix = newMatrix;
        }

        public void PopWorldMatrix()
        {
            WorldMatrix = m_worldMatrixStack.Pop();
        }

        public abstract IVertexBuffer<T> GetVertexBuffer<T>()
            where T : struct;

        public abstract void Activate(IVertexBuffer buffer);

        public abstract void Activate(IShader shader);

        public abstract void DrawArrays(PrimitiveType primitive, int vertexOffset, int vertexCount);
    }
}
