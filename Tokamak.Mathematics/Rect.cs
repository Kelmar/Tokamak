using System;
using System.Runtime.CompilerServices;

using Silk.NET.Maths;

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

        public Rect(Point location, Point extent)
        {
            Location = location;
            Size = extent;
        }

        public Rect(int x, int y, int width, int height)
        {
            Location = new Point(x, y);
            Size = new Point(width, height);
        }

        public Point Location { get; set; }

        public Point Size { get; set; }

        public int Left => Location.X;

        public int Top => Location.Y;

        public int Right => Location.X + Size.X;

        public int Bottom => Location.Y + Size.Y;

        /// <summary>
        /// Gets a boolean indicating if this rectangle is valid or not.
        /// </summary>
        public bool IsValid => Size.X > 0 && Size.Y > 0;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Point p) => (Left >= p.X) && (Top >= p.Y) && (p.X <= Right) && (p.Y <= Bottom);

        public static implicit operator Rectangle<int>(in Rect r) => new Rectangle<int>(r.Location, r.Size);
        public static implicit operator Rect(in Rectangle<int> r) => new Rect(r.Origin, r.Size);

    }
}
