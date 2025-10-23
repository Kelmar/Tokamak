using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

            float omd = 1 - delta;

            return
                v1 * omd * omd +
                v2 * 2 * omd * delta +
                v3 * delta * delta;
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

            float omd = 1 - delta;

            return
                v1 * omd * omd +
                v2 * 2 * omd * delta +
                v3 * delta * delta;
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

            float omd = 1 - delta;

            return
                v1 * omd * omd * omd +
                v2 * 3 * omd * omd * delta +
                v3 * 3 * omd * delta * delta +
                v4 * delta * delta * delta;
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

            float omd = 1 - delta;

            return
                v1 * omd * omd * omd +
                v2 * 3 * omd * omd * delta +
                v3 * 3 * omd * delta * delta +
                v4 * delta * delta * delta;
        }

        /// <summary>
        /// Solves an arbitrarily complex Bézier curve with N points.
        /// </summary>
        /// <param name="vectors">List of vectors to process.</param>
        /// <param name="delta">The time delta from 0 to 1 to solve on the Bézier curve.</param>
        /// <remarks>
        /// The vector list must have at least 3 vectors to solve.
        /// 
        /// For lists of 3 or 4 vectors using <seealso cref="QuadSolve"/> and <seealso cref="CubicSolve"/> would be faster.
        /// </remarks>
        /// <returns>A point along the computed Bézier curve.</returns>
        public static Vector2 SolveN(IEnumerable<Vector2> vectors, float delta)
        {
            var allVectors = vectors.ToArray();
            Debug.Assert(allVectors.Length > 2, "Invalid Bézier curve");

            // We only call the QuadSolve() and CubicSolve() in release builds.
            // This is so we can use this method to help validate unit tests of the above functions.
#if !DEBUG
            // Use unrolled versions if available.
            if (allVectors.Length == 3)
                return QuadSolve(allVectors[0], allVectors[1], allVectors[2], delta);
            else if (allVectors.Length == 4)
                return CubicSolve(allVectors[0], allVectors[1], allVectors[2], allVectors[3], delta);
#endif

            delta = Math.Clamp(delta, 0, 1);

            float omd = 1 - delta;

            Vector2 sum = Vector2.Zero;

            for (int i = 0; i < allVectors.Length; i++)
                sum += allVectors[i] * MathF.Pow(omd, allVectors.Length - i) * MathF.Pow(delta, i);

            return sum;
        }

        /// <summary>
        /// Solves an arbitrarily complex 3D Bézier curve with N points.
        /// </summary>
        /// <param name="vectors">List of vectors to process.</param>
        /// <param name="delta">The time delta from 0 to 1 to solve on the Bézier curve.</param>
        /// <remarks>
        /// The vector list must have at least 3 vectors to solve.
        /// 
        /// For lists of 3 or 4 vectors using <seealso cref="QuadSolve"/> and <seealso cref="CubicSolve"/> would be faster.
        /// </remarks>
        /// <returns>A point along the computed Bézier curve.</returns>
        public static Vector3 SolveN(IEnumerable<Vector3> vectors, float delta)
        {
            var allVectors = vectors.ToArray();
            Debug.Assert(allVectors.Length > 2, "Invalid Bézier curve");

            // We only call the QuadSolve() and CubicSolve() in release builds.
            // This is so we can use this method to help validate unit tests of the above functions.
#if !DEBUG
            // Use unrolled versions if available.
            if (allVectors.Length == 3)
                return QuadSolve(allVectors[0], allVectors[1], allVectors[2], delta);
            else if (allVectors.Length == 4)
                return CubicSolve(allVectors[0], allVectors[1], allVectors[2], allVectors[3], delta);
#endif

            delta = Math.Clamp(delta, 0, 1);

            float omd = 1 - delta;

            Vector3 sum = Vector3.Zero;

            for (int i = 0; i < allVectors.Length; i++)
                sum += allVectors[i] * MathF.Pow(omd, allVectors.Length - i) * MathF.Pow(delta, i);

            return sum;
        }
    }
}
