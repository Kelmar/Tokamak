using System.Numerics;

namespace Tokamak.Graphite
{
    public static class PathEx
    {
        public static void MoveTo(this IPath path, float x, float y) =>
            path.MoveTo(new Vector2(x, y));

        public static void LineTo(this IPath path, float x, float y) =>
            path.LineTo(new Vector2(x, y));
    }
}
