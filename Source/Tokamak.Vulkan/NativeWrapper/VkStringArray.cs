using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using Silk.NET.Core.Native;

namespace Tokamak.Vulkan.NativeWrapper
{
    /// <summary>
    /// Managed wrapper around an unmanaged array of strings.
    /// </summary>
    internal unsafe class VkStringArray : IDisposable
    {
        public VkStringArray(IEnumerable<string> values)
        {
            Values = values.ToArray();

            if (Values.Length > 0)
                Pointer = (byte**)SilkMarshal.StringArrayToPtr(Values);
            else
                Pointer = null;
        }

        public void Dispose()
        {
            if (Pointer != null)
                SilkMarshal.Free((nint)Pointer);

            GC.SuppressFinalize(this);
        }

        public string[] Values { get; }

        public uint Length => (uint)Values.Length;

        public byte** Pointer { get; }
    }
}
