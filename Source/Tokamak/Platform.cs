﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Buffer;
using Tokamak.Formats;
using Tokamak.Mathematics;

namespace Tokamak
{
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

        public abstract IVertexBuffer<T> GetVertexBuffer<T>(BufferType type)
            where T : unmanaged;

        public abstract ITextureObject GetTextureObject(PixelFormat format, Point size);

        public abstract IElementBuffer GetElementBuffer(BufferType type);

        protected abstract IPipelineFactory GetPipelineFactory(PipelineConfig config);

        protected virtual void ValidatePipelineConfig(PipelineConfig config)
        {
            if (config.InputFormat == null)
                throw new Exception("InputFormat not specified, call UseInputFormat().");
        }

        public IPipeline GetPipeline(Action<PipelineConfig> configurator)
        {
            PipelineConfig config = new PipelineConfig();
            configurator(config);

            ValidatePipelineConfig(config);

            using var factory = GetPipelineFactory(config);
            return factory.Build();
        }
    }
}
