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

        public void Render(IFontRenderer renderer)
        {
            switch (Points.Count)
            {
            case 2:
                renderer.DrawLine(Points[0], Points[1]);
                break;

            case 3:
                renderer.DrawQuadradic(Points[0], Points[1], Points[2]);
                break;

            case 4:
                renderer.DrawCubic(Points[0], Points[1], Points[2], Points[3]);
                break;
            }
        }
    }
}
