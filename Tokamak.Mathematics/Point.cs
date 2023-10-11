using System.Numerics;

using Silk.NET.Maths;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Represents an integer point in a 2D Cartesian coordinate system.
    /// </summary>
    public struct Point
    {
        private static Point s_zero = new Point(0, 0);

        private static Point s_one = new Point(1, 1);

        public static ref readonly Point Zero => ref s_zero;

        public static ref readonly Point One => ref s_one;

        public Point()
        {
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public int[] ToArray() { return new int[] { X, Y }; }

        public override string ToString() => $"({X},{Y})";

        // More specific to less specific, require a cast.
        public static explicit operator Point(Vector2 v) => new Point((int)v.X, (int)v.Y);
        public static explicit operator Point(Vector3 v) => new Point((int)v.X, (int)v.Y);
        public static explicit operator Point(Vector4 v) => new Point((int)v.X, (int)v.Y);

        // Less specific to more specific, no cast required.
        public static implicit operator Vector2(Point p) => new Vector2(p.X, p.Y);
        public static implicit operator Vector3(Point p) => new Vector3(p.X, p.Y, 0);
        public static implicit operator Vector4(Point p) => new Vector4(p.X, p.Y, 0, 1);

        public static implicit operator Point(Vector2D<int> p) => new Point(p.X, p.Y);
        public static implicit operator Vector2D<int>(Point p) => new Vector2D<int>(p.X, p.Y);
    }
}
