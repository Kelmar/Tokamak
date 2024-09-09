using System;

namespace Tokamak.Tritium
{
    [Flags]
    public enum GlobalBuffer
    {
        ColorBuffer = 0x0001,
        DepthBuffer = 0x0002,
        StencilBuffer = 0x0004,
    }
}
