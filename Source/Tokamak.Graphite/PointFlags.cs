using System;

namespace Tokamak.Graphite
{
    [Flags]
    internal enum PointFlags
    {
        None       = 0x0000,
        Corner     = 0x0001,
        Left       = 0x0002,
        OuterBevel = 0x0004,
        InnerBevel = 0x0008
    }
}
