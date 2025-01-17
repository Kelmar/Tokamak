using System;
using System.Numerics;

namespace Tokamak.Quill.Readers.TTF.Tables
{
    /// <summary>
    /// Parses the header information of the TTF file along with the table lookup entries.
    /// </summary>
    /// <remarks>
    /// This also checks for and loads either "bhed" (bitmap) or "head" (vector) and sets the
    /// any flags needed for loading additional tables.
    /// </remarks>
    internal static class Header
    {
        private const UInt32 MAGIC_NUMBER = 0x5F0F3CF5;

        private static void ParseHeader(ParseState state)
        {
            UInt16 majorVersion = state.ReadUInt16();
            UInt16 minorVersion = state.ReadUInt16();

            UInt16 fontRevMajor = state.ReadUInt16();
            UInt16 fontRevMinor = state.ReadUInt16();

            state.Version = new Version(majorVersion, minorVersion);
            state.Revision = new Version(fontRevMajor, fontRevMinor);

            state.ChecksumAdjust = state.ReadUInt32();

            // Magic number check.
            UInt32 magicNumber = state.ReadUInt32();

            if (magicNumber != MAGIC_NUMBER)
                throw new FontFileException("Invalid magic number");

            state.Flags = state.ReadUInt16();

            state.UnitsPerEm = state.ReadUInt16();

            state.Created = state.ReadLongDateTime();
            state.Modified = state.ReadLongDateTime();

            state.Min = new Vector2(state.ReadInt16(), state.ReadInt16());
            state.Max = new Vector2(state.ReadInt16(), state.ReadInt16());

            MacStyle macStyle = (MacStyle)state.ReadUInt16();

            state.LowestRecPPEM = state.ReadUInt16();

            state.FontDirectionHint = state.ReadInt16();

            state.LocationFormat = state.ReadInt16();

            //int glyphDataFormat = Input.ReadInt16(); // Should be 0
        }

        private static void LoadTableEntries(ParseState state, int tableCount)
        {
            for (int i = 0; i < tableCount; ++i)
            {
                var entry = TableEntry.LoadFrom(state);
                state.Tables[entry.Tag] = entry;
            }
        }

        public static void Load(ParseState state)
        {
            var offsetTable = OffsetTable.LoadFrom(state);

            LoadTableEntries(state, offsetTable.NumTables);

            /*
             * Apple documentation says that the font renderer there uses the existance
             * of the bhed table as a flag to indicate if the font is a bitmap or vector
             * based font.
             */
            TableEntry entry = state.JumpToEntry("bhed");

            if (entry != null)
                state.Format = FontFormat.Bitmap;
            else
            {
                // "bhed" not found, use "head"
                entry = state.JumpToEntryOrFail("head");
                state.Format = FontFormat.Vector;
            }

            ParseHeader(state);
        }
    }
}
