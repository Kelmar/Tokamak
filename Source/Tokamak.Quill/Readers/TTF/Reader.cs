using System.IO;

using Tokamak.Mathematics;

using Tokamak.Quill.Readers.TTF.Tables;

namespace Tokamak.Quill.Readers.TTF
{
    public static class Reader
    {
        private static ParseState ReadStream(Stream input)
        {
            var state = new ParseState(input);

            // Header info is going to dictate how we load everything else.
            Header.Load(state);

            NameTable.Load(state);
            MaxProfile.Load(state);

            CharMap.Load(state);

            if (state.HasEntry("loca"))
            {
                // TrueType font
                LocationMap.Load(state);
                GlyphTable.Load(state);
            }
            else
            {
                // OpenType font?
                throw new FontFileException("OpenType not yet supported.");
            }

            return state;
        }

        public static Font LoadFrom(Stream input, float pointSize, in Point DPI)
        {
            ParseState state = ReadStream(input);

            var translator = new Translator(state, DPI);

            return translator.Get(pointSize, DPI);
        }
    }
}
