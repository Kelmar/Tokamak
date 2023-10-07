using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak
{
    public interface IVertexBuffer : IDisposable
    {
    }

    public interface IVertexBuffer<T> : IVertexBuffer
        where T : struct
    {
        void Set(IEnumerable<T> data);
    }
}
