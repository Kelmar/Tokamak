using System.Numerics;

namespace Tokamak.Mathematics
{
    public struct Point
    {
        public Point()
        {
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(Vector2 vector)
        {
            X = (int)vector.X;
            Y = (int)vector.Y;
        }

        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public int[] ToArray() { return new int[] { X, Y }; }

        public override string ToString() => $"({X},{Y})";
    }
}
