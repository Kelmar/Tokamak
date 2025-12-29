using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Represents an integer point in a 2D Cartesian coordinate system.
    /// </summary>
    public struct Point
    {
        #region Pseudo Constant Points

        private static Point s_zero = new Point(0, 0);
        private static Point s_unit = new Point(1, 1);
        private static Point s_unitX = new Point(1, 0);
        private static Point s_unitY = new Point(0, 1);
        
        /// <summary>
        /// A readonly point that lies at the origin.
        /// </summary>
        public static ref readonly Point Zero => ref s_zero;

        /// <summary>
        /// A readonly point that lies at (1, 1)
        /// </summary>
        public static ref readonly Point Unit => ref s_unit;

        /// <summary>
        /// A readonly point that lies at (1, 0)
        /// </summary>
        public static ref readonly Point UnitX => ref s_unitX;

        /// <summary>
        /// A readonly point that lies at (0, 1)
        /// </summary>
        public static ref readonly Point UnitY => ref s_unitY;

        #endregion Pseudo Constant Points

        /// <summary>
        /// Constructs a default point.
        /// </summary>
        public Point()
        {
        }

        /// <summary>
        /// Constructs a point with the given X and Y values.
        /// </summary>
        /// <param name="x">The X value to use.</param>
        /// <param name="y">The Y value to use.</param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets or sets the X value of the point.
        /// </summary>
        public int X { readonly get; set; } = 0;

        /// <summary>
        /// Gets or sets teh Y value of the point.
        /// </summary>
        public int Y { readonly get; set; } = 0;

        /// <summary>
        /// Converts the point to an array.
        /// </summary>
        /// <returns>A two element array where X is the first value an Y is the second.</returns>
        public readonly int[] ToArray() => [X, Y];

        /// <inheritdoc />
        public override readonly string ToString() => $"({X},{Y})";

        /// <inheritdoc />
        public override readonly bool Equals([NotNullWhen(true)] object? obj) => (obj is Point p) && Equals(p);

        public readonly bool Equals(in Point other) => this == other;

        /// <inheritdoc />
        public override readonly int GetHashCode() => HashCode.Combine(X, Y);

        /// <summary>
        /// Makes a new point from the current point offset by the specified x and y values.
        /// </summary>
        public Point Offset(int x, int y)
        {
            return new Point(X + x, Y + y);
        }

        /// <summary>
        /// Translates the Vector2 to a Point by rounding up.
        /// </summary>
        public static Point Ceiling(in Vector2 v)
        {
            return new Point((int)MathF.Ceiling(v.X), (int)MathF.Ceiling(v.Y));
        }

        /// <summary>
        /// Translates the Vector2 to a Point by rounding down.
        /// </summary>
        public static Point Floor(in Vector2 v)
        {
            return new Point((int)MathF.Floor(v.X), (int)MathF.Floor(v.Y));
        }

        /// <summary>
        /// Translates the Vector2 to a Point by rounding.
        /// </summary>
        public static Point Round(in Vector2 v)
        {
            return new Point((int)MathF.Round(v.X), (int)MathF.Round(v.Y));
        }

        #region Casts

        // More specific to less specific, require a cast.
        public static explicit operator Point(Vector2 v) => new Point((int)v.X, (int)v.Y);
        public static explicit operator Point(Vector3 v) => new Point((int)v.X, (int)v.Y);
        public static explicit operator Point(Vector4 v) => new Point((int)v.X, (int)v.Y);

        // Less specific to more specific, no cast required.
        public static implicit operator Vector2(Point p) => new Vector2(p.X, p.Y);
        public static implicit operator Vector3(Point p) => new Vector3(p.X, p.Y, 0);
        public static implicit operator Vector4(Point p) => new Vector4(p.X, p.Y, 0, 1);

        #endregion Casts

        public static Point operator +(in Point p) => p;

        public static Point operator -(in Point p) => new Point(-p.X, -p.Y);

        public static Point operator +(in Point lhs, in Point rhs) => new Point(lhs.X + rhs.X, lhs.Y + rhs.Y);

        public static Point operator -(in Point lhs, in Point rhs) => new Point(lhs.X - rhs.X, lhs.Y - rhs.Y);

        public static Point operator *(int lhs, in Point rhs) => new Point(lhs * rhs.X, lhs * rhs.Y);

        public static Point operator *(in Point lhs, int rhs) => new Point(lhs.X * rhs, lhs.Y * rhs);

        public static Vector2 operator *(float lhs, in Point rhs) => new Vector2(lhs * rhs.X, lhs * rhs.Y);

        public static Vector2 operator *(in Point lhs, float rhs) => new Vector2(lhs.X * rhs, lhs.Y * rhs);

        public static Point operator /(in Point lhs, int rhs) => new Point(lhs.X / rhs, lhs.Y / rhs);

        public static Vector2 operator /(in Point lhs, float rhs) => new Vector2(lhs.X / rhs, lhs.Y / rhs);

        public static bool operator ==(in Point lhs, in Point rhs) => (lhs.X == rhs.X && lhs.Y == rhs.Y);

        public static bool operator !=(in Point lhs, in Point rhs) => (lhs.X != rhs.X || lhs.Y != rhs.Y);
    }
}
