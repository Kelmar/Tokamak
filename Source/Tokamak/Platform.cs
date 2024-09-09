using System;
using System.Collections.Generic;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak
{
    [Obsolete("Migrating to Tritium IAPILayer")]
    public abstract class Platform : IDisposable
    {
        private readonly Stack<Matrix4x4> m_worldMatrixStack = new Stack<Matrix4x4>();

        protected Platform()
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
    }
}
