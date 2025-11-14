using Silk.NET.Maths;

namespace Tokamak.Mathematics.Silk
{
    public static class PointX
    {
        extension (Point p)
        {
            public static Point FromSilk(in Vector2D<int> v) => new Point(v.X, v.Y);
        }

        public static Point ToPoint(this in Vector2D<int> v) => new Point(v.X, v.Y);

        public static Vector2D<int> ToSilkVector(this in Point p) => new Vector2D<int>(p.X, p.Y);
    }
}
