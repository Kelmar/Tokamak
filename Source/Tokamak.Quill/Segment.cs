using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Quill
{
    /// <summary>
    /// Section of a loop.
    /// </summary>
    public class Segment
    {
        public List<Vector2> Points { get; set; } = new();

        public void Render(IFontRenderer renderer, Vector2 scale)
        {
            switch (Points.Count)
            {
            case 2:
                renderer.DrawLine(Points[0] * scale, Points[1] * scale);
                break;

            case 3:
                renderer.DrawQuadradic(Points[0] * scale, Points[1] * scale, Points[2] * scale);
                break;

            case 4:
                renderer.DrawCubic(Points[0] * scale, Points[1] * scale, Points[2] * scale, Points[3] * scale);
                break;
            }
        }
    }
}
