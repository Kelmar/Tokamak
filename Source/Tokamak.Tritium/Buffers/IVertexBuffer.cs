using System;

using Tokamak.Tritium.APIs;

namespace Tokamak.Tritium.Buffers
{
    public interface IVertexBuffer<T> : IDeviceResource
        where T : unmanaged
    {
        /// <summary>
        /// Set this as the active vertex buffer object.
        /// </summary>
        void Activate();

        /// <summary>
        /// Returns if the vertex buffer object has data in it or not.
        /// </summary>
        /// <remarks>
        /// If true, then this vertex buffer has no data in it.
        /// </remarks>
        bool IsEmpty { get; }

        /// <summary>
        /// Send vertex data to the video driver.
        /// </summary>
        /// <param name="data">Data to send to the display driver.</param>
        void Set(in ReadOnlySpan<T> data);
    }
}
