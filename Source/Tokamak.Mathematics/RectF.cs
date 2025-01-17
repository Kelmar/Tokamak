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
            Location = Vector2.Zero;
            Size = Vector2.Zero;
        }

        public RectF(in Vector2 location, in Vector2 extent)
        {
            Location = location;
            Size = extent;
        }

        public RectF(float x, float y, float width, float height)
        {
            Location = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        public Vector2 Location { get; set; }

        public Vector2 Size { get; set; }

        public float Left => Location.X;

        public float Top => Location.Y;

        public float Right => Location.X + Size.X;

        public float Bottom => Location.Y + Size.Y;

        /// <summary>
        /// Gets a boolean indicating if this rectangle is valid or not.
        /// </summary>
        public bool IsValid => (Size.X > 0) || (Size.Y > 0);

        /// <summary>
        /// Get the rectangle that intersects with the supplied rectangle and this rectangle.
        /// </summary>
        /// <param name="clipRect">The rectangle to get the intersection of.</param>
        /// <returns>An intersecting rectangle.</returns>
        public RectF Intersect(in RectF clipRect)
        {
            float x = Math.Min(Right - 1, clipRect.Right - 1);
            float y = Math.Min(Bottom - 1, clipRect.Bottom - 1);

            float w = x - Location.X + 1;
            float h = y - Location.Y + 1;

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
            float w = Size.X - 2 * by.X;
            float h = Size.Y - 2 * by.Y;

            return new RectF(x, y, w, h);
        }

        /// <summary>
        /// Tests to see if the given point falls inside the rectangle.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Vector2 v) => (Left >= v.X) && (Top >= v.Y) && (v.X <= Right) && (v.Y <= Bottom);

        public override string ToString() => $"<{Left},{Top}>-<{Right},{Bottom}>";

        public static RectF FromCoordinates(in Vector2 v1, in Vector2 v2)
        {
            float x = MathF.Min(v1.X, v2.X);
            float y = MathF.Min(v1.Y, v2.Y);

            float w = MathF.Max(v1.X, v2.X) - x;
            float h = MathF.Max(v1.Y, v2.Y) - y;

            return new RectF(x, y, w, h);
        }
    }
}
