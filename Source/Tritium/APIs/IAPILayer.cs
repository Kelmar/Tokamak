using System;
using System.Collections.Generic;

using Tokamak.Core.Utilities;

using Tokamak.Mathematics;

using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;
using Tokamak.Tritium.Pipelines;

namespace Tokamak.Tritium.APIs
{
    public interface IAPILayer : IDisposable
    {
        /// <summary>
        /// Called when the display area is resized.
        /// </summary>
        /// <remarks>
        /// This is usually for when running under a Window and the user adjusts the window size.
        /// </remarks>
        event SimpleEvent<Point> OnResize;

        /// <summary>
        /// Called to render to the view.
        /// </summary>
        event SimpleEvent<double> OnRender;

        /// <summary>
        /// Get the current size of the view port in pixels.
        /// </summary>
        Point ViewBounds { get; }

        /// <summary>
        /// Enumerate the connected displays on a host system.
        /// </summary>
        IEnumerable<Monitor> GetMonitors();

        /// <summary>
        /// Swap front/back buffers if buffering is enabled.
        /// </summary>
        void SwapBuffers();

        /// <summary>
        /// Factory method for getting a command list.
        /// </summary>
        /// <returns>A new command list.</returns>
        ICommandList CreateCommandList();

        /// <summary>
        /// Gets a factory that will build a new pipeline.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        IFactory<IPipeline> GetPipelineFactory(PipelineConfig config);

        IVertexBuffer<T> GetVertexBuffer<T>(BufferUsage usage)
            where T : unmanaged;

        IElementBuffer GetElementBuffer(BufferUsage usage);

        ITextureObject GetTextureObject(PixelFormat format, Point size);
    }
}
