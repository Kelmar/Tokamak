using System.Collections.Generic;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Quill
{
    public class SimpleGlyph : IGlyph
    {
        private List<Loop> m_loops = new();

        public Vector2 Scale { get; init; }

        public RectF Bounds { get; init; }

        public int Index { get; init; }

        public Point Bearing => Point.Unit;

        public Vector2 Advance => Vector2.One;

        public List<Loop> Loops
        {
            get => m_loops;
            set => m_loops = value ?? new();
        }

        public void Render(IFontRenderer renderer)
        {
            foreach (var loop in Loops)
                loop.Render(renderer, Scale);
        }
    }
}
