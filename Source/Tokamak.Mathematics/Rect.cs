using System;
using System.Diagnostics.CodeAnalysis;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Rectangle in a 2D drawing grid.
    /// </summary>
    public struct Rect
    {
        private static readonly Rect s_zero = new Rect(0, 0, 0, 0);
        private static readonly Rect s_one = new Rect(0, 0, 1, 1);

        /// <summary>
        /// Reference empty rectangle.
        /// </summary>
        public static ref readonly Rect Zero => ref s_zero;

        /// <summary>
        /// Reference unit sized rectangle.
        /// </summary>
        public static ref readonly Rect One => ref s_one;

        public Rect()
        {
        }

        public Rect(Point location, Point size)
        {
            Location = location;
            Size = size;
        }

        public Rect(int x, int y, int width, int height)
        {
            Location = new Point(x, y);
            Size = new Point(width, height);
        }

        public Point Location { readonly get; set; }

        public Point Size { readonly get; set; }

        public readonly int Left => Location.X;

        public readonly int Top => Location.Y;

        public readonly int Right => Location.X + Size.X;

        public readonly int Bottom => Location.Y + Size.Y;

        /// <summary>
        /// Checks if the rectangle is less than or equal to a zero size.
        /// </summary>
        public readonly bool IsEmpty => (Size.X <= 0) || (Size.Y <= 0);

        /// <summary>
        /// Get the rectangle that intersects with the supplied rectangle and this rectangle.
        /// </summary>
        /// <param name="clipRect">The rectangle to get the intersection of.</param>
        /// <returns>An intersecting rectangle.</returns>
        public Rect Intersect(in Rect clipRect)
        {
            int x = Math.Min(Right - 1, clipRect.Right - 1);
            int y = Math.Min(Bottom - 1, clipRect.Bottom - 1);

            int w = x - Location.X + 1;
            int h = y - Location.Y + 1;

            return new Rect(x, y, w, h);
        }

        /// <summary>
        /// Shrink/Grow a rectangle by a given amount.
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public Rect Inset(in Point by)
        {
            int x = Left + by.X;
            int y = Top + by.Y;
            int w = Size.X - 2 * by.X;
            int h = Size.Y - 2 * by.Y;

            return new Rect(x, y, w, h);
        }

        /// <summary>
        /// Tests to see if the given point falls inside the rectangle.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public readonly bool Contains(in Point p) => (Left >= p.X) && (Top >= p.Y) && (p.X <= Right) && (p.Y <= Bottom);

        /// <inheritdoc />
        public override readonly string ToString() => $"<{Left},{Top}>-<{Right},{Bottom}>";

        /// <inheritdoc />
        public override readonly bool Equals([NotNullWhen(true)] object? obj) => (obj is Rect r) && Equals(r);

        public readonly bool Equals(in Rect other) => this == other;

        /// <inheritdoc />
        public override readonly int GetHashCode() => HashCode.Combine(Left, Top, Size.X, Size.Y);

        public static Rect FromCoordinates(in Point p1, in Point p2)
        {
            int x = Math.Min(p1.X, p2.X);
            int y = Math.Min(p1.Y, p2.Y);

            int w = Math.Max(p1.X, p2.X) - x;
            int h = Math.Max(p1.Y, p2.Y) - y;

            return new Rect(x, y, w, h);
        }

        public static Rect operator +(in Rect r, in Point p)
        {
            Rect rval = r;
            rval.Location += p;
            return rval;
        }

        public static Rect operator -(in Rect r, in Point p)
        {
            Rect rval = r;
            rval.Location -= p;
            return rval;
        }

        #region Casts

        // More specific to less specific, require a cast.
        public static explicit operator Rect(in RectF r) => new Rect((Point)r.Location, (Point)r.Size);

        // Less specific to more specific, no cast requried.
        public static implicit operator RectF(in Rect r) => new RectF(r.Location, r.Size);

        #endregion Casts

        public static bool operator ==(in Rect lhs, in Rect rhs) =>
            lhs.Left == rhs.Left && lhs.Top == rhs.Top && lhs.Size.X == rhs.Size.X && lhs.Size.Y == rhs.Size.Y;

        public static bool operator !=(in Rect lhs, in Rect rhs) =>
            lhs.Left != rhs.Left || lhs.Top != rhs.Top || lhs.Size.X != rhs.Size.X || lhs.Size.Y != rhs.Size.Y;
    }
}
