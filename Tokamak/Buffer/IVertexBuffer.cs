using System;
using System.Collections.Generic;

namespace Tokamak.Buffer
{
    public interface IVertexBuffer<T> : IDeviceResource
        where T : struct
    {
        void Set(IEnumerable<T> data);
    }
}
