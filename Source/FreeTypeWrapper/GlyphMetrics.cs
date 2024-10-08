﻿using System.Numerics;

using Tokamak.Mathematics;

namespace FreeTypeWrapper
{
    /// <summary>
    /// Sizing of a specific glyph in the font face.
    /// </summary>
    public struct GlyphMetrics
    {
        /// <summary>
        /// Gets the actual dimensions of the rendered bitmap.
        /// </summary>
        public Point BitSize { get; set; }

        public Vector2 Size { get; set; }

        public Vector2 Advance { get; set; }

        public Vector2 HorizontalBearing { get; set; }

        public Vector2 VerticalBearing { get; set; }

        public float VerticalAdvance { get; set; }
    }
}
