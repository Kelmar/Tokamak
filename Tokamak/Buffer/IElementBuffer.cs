using System;

namespace Tokamak.Buffer
{
    public interface IElementBuffer : IDeviceResource
    {
        void Set(in ReadOnlySpan<uint> values);
    }
}
