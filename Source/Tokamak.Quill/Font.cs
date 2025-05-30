﻿using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Quill
{
    public class Font
    {
        internal Font(ICharacterMapper charMapper)
        {
            CharMapper = charMapper;
        }

        public required string FontId { get; init; }

        public required string Family { get; init; }

        public required string Subfamily { get; init; }

        /// <summary>
        /// Gets the spacing between each line of the font.
        /// </summary>
        public int LineSpacing { get; init; }

        /// <summary>
        /// The point size for this font
        /// </summary>
        public required float Points { get; init; }

        internal ICharacterMapper CharMapper { get; init; }

        /// <summary>
        /// Scaling factor based on ouput DPI, UnitsPerEm, and requested point size.
        /// </summary>
        internal Vector2 Scale { get; init; }

        public required IReadOnlyList<IGlyph> Glyphs { get; init; }

        public int GetGlyphIdFor(char c)
        {
            return CharMapper.MapChar(c);
        }

        public IGlyph GetGlyphFor(char c)
        {
            int id = CharMapper.MapChar(c);
            return Glyphs[id];
        }
    }
}
