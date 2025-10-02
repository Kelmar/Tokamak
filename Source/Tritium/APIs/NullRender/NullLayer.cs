using System;
using System.Collections.Generic;

using Tokamak.Utilities;

using Tokamak.Mathematics;

using Tokamak.Hosting.Abstractions;

using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;
using Tokamak.Tritium.Pipelines;

namespace Tokamak.Tritium.APIs.NullRender
{
    /// <summary>
    /// Dummy layer that doesn't actually do anything.
    /// </summary>
    internal sealed class NullLayer : ITick, IAPILayer
    {
        // These events are not called from this layer
#pragma warning disable 00067
        public event SimpleEvent<Point> OnResize;
        public event SimpleEvent<double> OnRender;
#pragma warning restore 00067

        public event SimpleEvent OnLoad;

        private bool m_firstTick = true;

        public NullLayer()
        {
        }

        public void Dispose()
        {
        }

        public Point ViewBounds { get; } = new Point(640, 480);

        public IEnumerable<Monitor> GetMonitors()
        {
            yield break;
        }

        public void SwapBuffers()
        {
        }

        public ICommandList CreateCommandList() => new NullCommandList();

        public IFactory<IPipeline> GetPipelineFactory(PipelineConfig config)
        {
            throw new NotImplementedException();
        }

        public IVertexBuffer<T> GetVertexBuffer<T>(BufferUsage usage) where T : unmanaged
        {
            throw new NotImplementedException();
        }

        public IElementBuffer GetElementBuffer(BufferUsage usage)
        {
            throw new NotImplementedException();
        }

        public ITextureObject GetTextureObject(PixelFormat format, Point size)
        {
            throw new NotImplementedException();
        }

        public void Tick()
        {
            if (m_firstTick)
            {
                m_firstTick = false;
                OnLoad?.Invoke();
            }
        }
    }
}
