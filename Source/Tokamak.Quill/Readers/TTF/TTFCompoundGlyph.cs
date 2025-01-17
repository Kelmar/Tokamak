using System.Collections.Generic;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Quill.Readers.TTF
{
    internal class TTFCompoundGlyph : ITTFGlyph
    {
        public required int Index { get; init; }

        public RectF Bounds { get; set; }

        /// <summary>
        /// List of child glyph information
        /// </summary>
        public List<ChildGlyphInfo> Children { get; } = new();

        /// <summary>
        /// List of TTF VM instructions.
        /// </summary>
        public List<byte> Instructions { get; set; } = new();
    }

    internal class ChildGlyphInfo
    {
        public int Index;

        public Matrix3x2 Transform;
    }
}
