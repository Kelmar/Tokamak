using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Tritium.APIs;

namespace Tokamak.Tritium.Buffers
{
    public interface IElementBuffer : IDeviceResource
    {
        void Activate();

        void Set(in ReadOnlySpan<uint> values);
    }
}
