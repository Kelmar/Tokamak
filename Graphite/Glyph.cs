using System;
using System.Numerics;

using Tokamak.Mathematics;

namespace Graphite
{
    /// <summary>
    /// Holds details about a glyph in a font face.
    /// </summary>
    public struct Glyph
    {
        /// <summary>
        /// The character for this glyph.
        /// </summary>
        public char Char;

        /// <summary>
        /// The cache sheet this glyph can be found on.
        /// </summary>
        public int SheetNumber;

        /// <summary>
        /// The size of the glyph in pixels
        /// </summary>
        public Point Size;

        /// <summary>
        /// The top left UV of the glyph.
        /// </summary>
        public Vector2 TopLeftUV;

        /// <summary>
        /// The bottom right UV of the glyph.
        /// </summary>
        public Vector2 BotRightUV;

        /// <summary>
        /// Indicates how the glyph should be repositioned to fit it's requested layout in pixels.
        /// </summary>
        public Point Bearing;

        /// <summary>
        /// Gets how the characters should be advanced along the screen.
        /// </summary>
        public Vector2 Advance;

        public override string ToString() => Char.ToString();
    }
}
