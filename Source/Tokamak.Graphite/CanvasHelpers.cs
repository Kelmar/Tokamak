using System.Numerics;

namespace Tokamak.Graphite
{
    public static class CanvasHelpers
    {
        public static void MoveTo(this ICanvas canvas, float x, float y)
            => canvas.MoveTo(new Vector2(x, y));

        public static void LineTo(this ICanvas canvas, float x, float y)
            => canvas.LineTo(new Vector2(x, y));
    }
}
