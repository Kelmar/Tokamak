using System;
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
            Monitors = EnumerateMonitors().ToList().AsReadOnly();
        }

        public virtual void Dispose()
        {
        }

        public Matrix4x4 WorldMatrix { get; set; }

        public Matrix4x4 ProjectionMatrix { get; set; }

        public Matrix4x4 ViewMatrix { get; set; }

        virtual public Rect Viewport { get; set; }

        public IReadOnlyList<Monitor> Monitors { get; }

        public void PushWorldMatrix(in Matrix4x4 newMatrix)
        {
            m_worldMatrixStack.Push(WorldMatrix);
            WorldMatrix = newMatrix;
        }

        protected IEnumerable<Monitor> EnumerateMonitors()
        {
            var platform = Silk.NET.Windowing.Window.GetWindowPlatform(false);

            if (platform == null)
                throw new Exception("Unable to get window platform.");

            var mainMonitor = platform.GetMainMonitor();

            foreach (var m in platform.GetMonitors())
            {
                // Silk doesn't return the DPI info yet, hard coded for now.

                yield return new Monitor
                {
                    Index = m.Index,
                    IsMain = m.Index == mainMonitor.Index,
                    Name = m.Name,
                    Gamma = m.Gamma,
                    DPI = new Point(192, 192),
                    RawDPI = new Vector2(192, 192),
                    WorkArea = m.Bounds
                };
            }
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

        public abstract ICommandList GetCommandList();
    }
}
