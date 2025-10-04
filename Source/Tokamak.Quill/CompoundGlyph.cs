using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Quill
{
    public class CompoundGlyph : IGlyph
    {
        private class ChildInfo
        {
            public required IGlyph Glyph { get; set; }

            public required Matrix3x2 Transform { get; set; }
        }

        private List<ChildInfo> m_children = new();

        public int Index { get; init; }

        public RectF Bounds { get; init; }

        public Point Bearing => Point.Unit;

        public Vector2 Advance => Vector2.One;

        public IEnumerable<IGlyph> Children => m_children.Select(ci => ci.Glyph);

        public void AddChild(IGlyph child, Matrix3x2 transform)
        {
            m_children.Add(new ChildInfo
            {
                Glyph = child,
                Transform = transform
            });
        }

        public void Render(IFontRenderer renderer)
        {
            foreach (var child in m_children)
            {
                var old = renderer.Transform;

                try
                {
                    renderer.Transform *= child.Transform;
                    child.Glyph.Render(renderer);
                }
                finally
                {
                    renderer.Transform = old;
                }
            }
        }
    }
}
