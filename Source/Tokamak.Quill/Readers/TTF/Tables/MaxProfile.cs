using System;

namespace Tokamak.Quill.Readers.TTF.Tables
{
    internal static class MaxProfile
    {
        public static void Load(ParseState state)
        {
            state.JumpToEntryOrFail("maxp");

            // Version 0.5 info
            UInt16 majorVersion = state.ReadUInt16();
            UInt16 minorVersion = state.ReadUInt16();

            state.GlyphCount = state.ReadUInt16();

            // MS docs say this is needed for TTF fonts, but we really haven't needed it.
#if false
            if (majorVersion < 1)
                return;

            // Version 1.0 info
            UInt16 maxPoints = state.ReadUInt16();
            UInt16 maxCountours = state.ReadUInt16();
            UInt16 maxCompositeCountours = state.ReadUInt16();
            UInt16 maxZones = state.ReadUInt16();
            UInt16 maxTwilightPoints = state.ReadUInt16();
            UInt16 maxStorage = state.ReadUInt16(); // Number of Storage Area locations
            UInt16 maxFunctionDefs = state.ReadUInt16();
            UInt16 maxInstructionDefs = state.ReadUInt16();
            UInt16 maxStackElements = state.ReadUInt16();
            UInt16 maxSizeOfInstructions = state.ReadUInt16();
            UInt16 maxComponentElements = state.ReadUInt16();
            UInt16 maxComponentDepth = state.ReadUInt16();
#endif
        }
    }
}
