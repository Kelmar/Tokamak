using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Buffer
{
    public interface IVertexBuffer : IDeviceResource
    {
    }

    public interface IVertexBuffer<T> : IVertexBuffer
        where T : struct
    {
        void Set(IEnumerable<T> data);
    }
}
