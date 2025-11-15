using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Rectangle in a 2D drawing grid.
    /// </summary>
    public struct RectF
    {

        private static readonly RectF s_zero = new RectF(0, 0, 0, 0);
        private static readonly RectF s_one = new RectF(0, 0, 1, 1);

        /// <summary>
        /// Reference empty rectangle.
        /// </summary>
        public static ref readonly RectF Zero => ref s_zero;

        /// <summary>
        /// Reference unit sized rectangle.
        /// </summary>
        public static ref readonly RectF One => ref s_one;

        public RectF()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;
        }

        public RectF(in Vector2 location, in Vector2 extent)
        {
            X = location.X;
            Y = location.Y;
            Width = extent.X;
            Height = extent.Y;
        }

        public RectF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public float Left => X;

        public float Top => Y;

        public float Right => X + Width;

        public float Bottom => Y + Height;

        public Vector2 TopLeft => new Vector2(Left, Top);

        public Vector2 TopRight => new Vector2(Right, Top);

        public Vector2 BottomLeft => new Vector2(Left, Bottom);

        public Vector2 BottomRight => new Vector2(Right, Bottom);

        /// <summary>
        /// Checks if the rectangle is less than or equal to a zero size.
        /// </summary>
        public bool IsEmpty => (Width <= 0) || (Width == float.NaN) || (Height <= 0) || (Height == float.NaN);

        /// <summary>
        /// Get the rectangle that intersects with the supplied rectangle and this rectangle.
        /// </summary>
        /// <param name="clipRect">The rectangle to get the intersection of.</param>
        /// <returns>An intersecting rectangle.</returns>
        public RectF Intersect(in RectF clipRect)
        {
            float x = Math.Min(Right - 1, clipRect.Right - 1);
            float y = Math.Min(Bottom - 1, clipRect.Bottom - 1);

            float w = x - X + 1;
            float h = y - Y + 1;

            return new RectF(x, y, w, h);
        }

        /// <summary>
        /// Shrink/Grow a rectangle by a given amount.
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public RectF Inset(in Vector2 by)
        {
            float x = Left + by.X;
            float y = Top + by.Y;
            float w = Width - 2 * by.X;
            float h = Width - 2 * by.Y;

            return new RectF(x, y, w, h);
        }

        /// <summary>
        /// Tests to see if the given point falls inside the rectangle.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool Contains(Vector2 v) => (Left >= v.X) && (Top >= v.Y) && (v.X <= Right) && (v.Y <= Bottom);

        public static RectF FromCoordinates(in Vector2 v1, in Vector2 v2)
        {
            float x = MathF.Min(v1.X, v2.X);
            float y = MathF.Min(v1.Y, v2.Y);

            float w = MathF.Max(v1.X, v2.X) - x;
            float h = MathF.Max(v1.Y, v2.Y) - y;

            return new RectF(x, y, w, h);
        }

        public override string ToString() => $"<{Left},{Top}>-<{Right},{Bottom}>";
    }
}
