using System;

using Tokamak.Tritium.APIs;

namespace Tokamak.Tritium.Buffers
{
    public interface IVertexBuffer<T> : IDeviceResource
        where T : unmanaged
    {
        void Activate();

        void Set(in ReadOnlySpan<T> data);
    }
}
