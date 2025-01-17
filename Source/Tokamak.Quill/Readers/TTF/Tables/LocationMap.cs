using System.Linq;

namespace Tokamak.Quill.Readers.TTF.Tables
{
    internal static class LocationMap
    {
        public static void Load(ParseState state)
        {
            state.JumpToEntryOrFail("loca");

            switch (state.LocationFormat)
            {
            case 0:
                state.GlyphOffsets = state.ReadUShorts(state.GlyphCount + 1).Select(s => (long)s << 1).ToList();
                break;

            case 1:
                state.GlyphOffsets = state.ReadWords(state.GlyphCount + 1).Select(i => (long)i).ToList();
                break;

            default:
                throw new FontFileException($"Unknown glyph offset format {state.LocationFormat}");
            }
        }
    }
}
