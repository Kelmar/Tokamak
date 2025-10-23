using System.Numerics;

namespace Tokamak.Graphite
{
    public static class PathEx
    {
        public static void MoveTo(this Path path, float x, float y) =>
            path.MoveTo(new Vector2(x, y));

        public static void LineTo(this Path path, float x, float y) =>
            path.LineTo(new Vector2(x, y));

        /// <summary>
        /// Add a circular arc to the the path.
        /// </summary>
        /// <param name="center">The center of the circle to arc through.</param>
        /// <param name="radius">The radius of the circle to draw.</param>
        /// <param name="start">The starting angle to draw at.</param>
        /// <param name="end">The angle to end drawing at.</param>
        public static void ArcTo(this Path path, in Vector2 center, float radius, float start, float end) =>
            path.ArcTo(center, new Vector2(radius, radius), start, end);
    }
}
