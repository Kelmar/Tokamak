using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Quill
{
    /// <summary>
    /// Holds a single loop for a glyph.
    /// </summary>
    public class Loop
    {
        private List<Segment> m_segments = new();

        internal List<Segment> Segments
        {
            get => m_segments;
            set => m_segments = value ?? new();
        }

        public void Render(IFontRenderer renderer, Vector2 scale)
        {
            foreach (var segment in Segments)
                segment.Render(renderer, scale);
        }
    }
}
