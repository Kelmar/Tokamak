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

        /// <summary>
        /// Returns if the element buffer object has data in it or not.
        /// </summary>
        /// <remarks>
        /// If true, then this element buffer has no data in it.
        /// </remarks>
        bool IsEmpty { get; }

        void Set(in ReadOnlySpan<uint> values);
    }
}
