using Tokamak.Mathematics;

namespace Tokamak.Quill
{
    public interface IGlyph
    {
        int Index { get; }

        RectF Bounds { get; }

        void Render(IFontRenderer renderer);
    }
}
