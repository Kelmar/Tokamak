using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Bezier math functions
    /// </summary>
    /// <remarks>
    /// References:
    /// https://en.wikipedia.org/wiki/B%C3%A9zier_curve
    /// https://www.youtube.com/watch?v=aVwxzDHniEw
    /// 
    /// In summary the Bernstein method is what you get if you carry out the
    /// DeCasteljau version of using lerp and reduce down to a large polynomial.
    /// 
    /// The DeCasteljau version is if you performed lerps on each of the line
    /// segments recursively.
    /// </remarks>
    public static class Bezier
    {
        /// <summary>
        /// Solves a Quadradic Bézier curve point using the Bernstein method.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="delta">Time over curve from 0 to 1></param>
        /// <returns>A point along the Bézier curve.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 QuadSolve(in Vector2 v1, in Vector2 v2, in Vector2 v3, float delta)
        {
            delta = Math.Clamp(delta, 0, 1);

            float sqDelta = delta * delta;

            return
                v1 * (delta - 1) * (delta - 1) +
                v2 * (2 * delta - 2 * sqDelta) +
                v3 * sqDelta;
        }

        /// <summary>
        /// Solves a Quadradic Bézier curve point using the Bernstein method in 3D space.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="delta">Time over curve from 0 to 1></param>
        /// <returns>A point along the Bézier curve.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3 QuadSolve(in Vector3 v1, in Vector3 v2, in Vector3 v3, float delta)
        {
            delta = Math.Clamp(delta, 0, 1);

            float sqDelta = delta * delta;

            return
                v1 * (delta - 1) * (delta - 1) +
                v2 * (2 * delta - 2 * sqDelta) +
                v3 * sqDelta;
        }

        /// <summary>
        /// Solves a Cubic Bézier curve point using the Bernstein method.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="delta">Time over curve from 0 to 1</param>
        /// <returns>A point along the Bézier curve.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 CubicSolve(in Vector2 v1, in Vector2 v2, in Vector2 v3, in Vector2 v4, float delta)
        {
            delta = Math.Clamp(delta, 0, 1);

            float cubeDelta = delta * delta * delta;
            float sqDelta = delta * delta;

            return
                v1 * (-cubeDelta + 3 * sqDelta - 3 * delta + 1) +
                v2 * (3 * cubeDelta - 6 * sqDelta + 3 * delta) +
                v3 * (-3 * cubeDelta + 3 * sqDelta) +
                v4 * (cubeDelta);
        }

        /// <summary>
        /// Solves a Cubic Bézier curve point using the Bernstein method in 3D space.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="delta">Time over curve from 0 to 1</param>
        /// <returns>A point along the Bézier curve.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3 CubicSolve(in Vector3 v1, in Vector3 v2, in Vector3 v3, in Vector3 v4, float delta)
        {
            delta = Math.Clamp(delta, 0, 1);

            float cubeDelta = delta * delta * delta;
            float sqDelta = delta * delta;

            return
                v1 * (-cubeDelta + 3 * sqDelta - 3 * delta + 1) +
                v2 * (3 * cubeDelta - 6 * sqDelta + 3 * delta) +
                v3 * (-3 * cubeDelta + 3 * sqDelta) +
                v4 * (cubeDelta);
        }
    }
}
