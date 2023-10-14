using System;
using System.Runtime.InteropServices;

namespace Tokamak.Vulkan
{
    internal unsafe sealed class VkString : IDisposable
    {
        public VkString(string value)
        {
            Value = value;
            Pointer = (byte*)Marshal.StringToHGlobalAnsi(Value);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((nint)Pointer);
            GC.SuppressFinalize(this);
        }

        public string Value { get; }

        public byte *Pointer { get; }
    }
}
