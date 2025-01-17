using System.Collections.Generic;

using Tokamak.Mathematics;

namespace Tokamak.Quill
{
    public class SimpleGlyph : IGlyph
    {
        private List<Loop> m_loops = new();

        public RectF Bounds { get; init; }

        public int Index { get; init; }

        public List<Loop> Loops
        {
            get => m_loops;
            set => m_loops = value ?? new();
        }

        public void Render(IFontRenderer renderer)
        {
            foreach (var loop in Loops)
                loop.Render(renderer);
        }
    }
}
