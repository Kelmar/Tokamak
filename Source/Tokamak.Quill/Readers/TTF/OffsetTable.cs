using System;

namespace Tokamak.Quill.Readers.TTF
{
    internal class OffsetTable
    {
        public UInt32 ScalarType { get; set; }

        public int NumTables { get; set; }

        public int SearchRange { get; set; }

        public int EntrySelector { get; set; }

        public int RangeShift { get; set; }

        public static OffsetTable LoadFrom(ParseState state)
        {
            return new OffsetTable
            {
                ScalarType = state.ReadUInt32(),
                NumTables = state.ReadUInt16(),
                SearchRange = state.ReadUInt16(),
                EntrySelector = state.ReadUInt16(),
                RangeShift = state.ReadUInt16()
            };
        }
    }
}
