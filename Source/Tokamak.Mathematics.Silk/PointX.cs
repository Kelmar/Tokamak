using Silk.NET.Maths;

namespace Tokamak.Mathematics.Silk
{
    public static class PointX
    {
        public static Point ToPoint(this in Vector2D<int> p) => new Point(p.X, p.Y);

        public static Vector2D<int> ToSilkVector(this in Point p) => new Vector2D<int>(p.X, p.Y);
    }
}
