using System;

namespace Tokamak.Quill.Readers.TTF
{
    [Flags]
    internal enum MacStyle : UInt16
    {
        Bold = 0x0001, // Bit 0
        Italic = 0x0002, // Bit 1
        Underline = 0x0004, // Bit 2
        Outline = 0x0008, // Bit 3
        Shadow = 0x0010, // Bit 4
        Condensed = 0x0020, // Bit 5
        Extended = 0x0040, // Bit 6

        Reserved = 0xFF80
    }
}
