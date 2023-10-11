using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FreeTypeWrapper
{
    /// <summary>
    /// Sizing information of a font face.
    /// </summary>
    public struct FontExtents
    {
        /// <summary>
        /// Gets the DPI that was used for calculating font sizes in pixels.
        /// </summary>
        public Vector2 DPI { get; set; }

        /// <summary>
        /// Get the conversion factor from EM units to pixel units.
        /// </summary>
        public float EmToPixel { get; set; }

        /// <summary>
        /// Gets the ascender size in EM units.
        /// </summary>
        /// <remarks>
        /// The ascender is the portion of a letter that extends above the top line.
        /// (E.g. the stem on a lowercase b or d)
        /// </remarks>
        public float AscenderEm { get; set; }

        /// <summary>
        /// Gets the ascender size in pixels.
        /// </summary>
        public int Ascender => (int)Math.Round(EmToPixel * AscenderEm);

        /// <summary>
        /// Gets the decsender size in EM units.
        /// </summary>
        public float DescenderEm { get; set; }

        /// <summary>
        /// Gets the descender size in pixels.
        /// </summary>
        /// <remarks>
        /// The descender is the part of the font that hangs below the baseline for some glyphs.
        /// (E.g. the stem of a lowercase p or g)
        /// </remarks>
        public int Descender => (int)Math.Round(EmToPixel * DescenderEm);

        /// <summary>
        /// Gets the line spacing in EM units
        /// </summary>
        public float LineSpacingEm { get; set; }

        /// <summary>
        /// Gets the line spacing in pixels.
        /// </summary>
        public int LineSpacing => (int)Math.Round(EmToPixel * LineSpacingEm);

        /// <summary>
        /// Gets the underline position in EM units.
        /// </summary>
        public float UnderlinePositionEm { get; set; }

        /// <summary>
        /// Gets the underline position in pixels.
        /// </summary>
        public int UnderlinePosition => (int)Math.Round(EmToPixel * UnderlinePositionEm);

        /// <summary>
        /// Gets the underline thickness in EM units.
        /// </summary>
        public float UnderlineThicknessEm { get; set; }

        /// <summary>
        /// Gets the underline thickness in pixels.
        /// </summary>
        public int UnderlineThickness => (int)Math.Round(EmToPixel * UnderlineThicknessEm);

        /// <summary>
        /// Get the minium size of a glyph for this font in EM units.
        /// </summary>
        public Vector2 MinEm { get; set; }

        /// <summary>
        /// Get the minimum size of a glyph for this font in pixels.
        /// </summary>
        public Vector2 Min => EmToPixel * MinEm;

        /// <summary>
        /// Get the maximum size of a glyph for this font in EM units.
        /// </summary>
        public Vector2 MaxEm { get; set; }

        /// <summary>
        /// Get the maximum size of a glyph for this font in pixels.
        /// </summary>
        public Vector2 Max => EmToPixel * MaxEm;

        /// <summary>
        /// Returns the largest size a bitmap will be for the font.
        /// </summary>
        public Vector2 MaxBitmap => Max - Min;
    }
}
