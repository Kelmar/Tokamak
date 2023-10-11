using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FreeTypeWrapper
{
    /// <summary>
    /// Sizing of a specific glyph in the font face.
    /// </summary>
    public struct GlyphMetrics
    {
        public Vector2 Size { get; set; }

        public Vector2 HorizontalBearing { get; set; }

        public Vector2 VerticalBearing { get; set; }

        public int VerticalAdvance { get; set; }
    }
}
