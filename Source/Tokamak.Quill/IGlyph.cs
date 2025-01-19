using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Quill
{
    public interface IGlyph
    {
        /// <summary>
        /// The glyph's index within it's parent font.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Overall bounding rectangle for the glyph
        /// </summary>
        RectF Bounds { get; }

        /// <summary>
        /// Indicates how the glyph should be repositioned to fit it's requested layout in pixels.
        /// </summary>
        Point Bearing { get; }

        /// <summary>
        /// Gets how the characters should be advanced along the screen.
        /// </summary>
        Vector2 Advance { get; }

        /// <summary>
        /// Draws the glyph to the provided rendering object.
        /// </summary>
        /// <param name="renderer"></param>
        void Render(IFontRenderer renderer);
    }
}
