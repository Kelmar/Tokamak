using System;

namespace Tokamak.Quill.Readers.TTF
{
    internal enum PlatformId : UInt16
    {
        Unicode = 0,
        Macintosh = 1,
        ISO = 2, // Deprecated
        Windows = 3,
        Custom = 4
    }
}
