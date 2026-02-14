using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Bézier math functions
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
        /// <param name="start"></param>
        /// <param name="control"></param>
        /// <param name="end"></param>
        /// <param name="delta">Time over curve from 0 to 1></param>
        /// <returns>A point along the Bézier curve.</returns>
        public static Vector2 QuadSolve(in Vector2 start, in Vector2 control, in Vector2 end, float delta)
        {
            delta = Math.Clamp(delta, 0, 1);

            float omd = 1 - delta;

            return
                start * omd * omd +
                control * 2 * omd * delta +
                end * delta * delta;
        }

        /// <summary>
        /// Solves a Quadradic Bézier curve point using the Bernstein method in 3D space.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="control"></param>
        /// <param name="end"></param>
        /// <param name="delta">Time over curve from 0 to 1></param>
        /// <returns>A point along the Bézier curve.</returns>
        public static Vector3 QuadSolve(in Vector3 start, in Vector3 control, in Vector3 end, float delta)
        {
            delta = Math.Clamp(delta, 0, 1);

            float omd = 1 - delta;

            return
                start * omd * omd +
                control * 2 * omd * delta +
                end * delta * delta;
        }

        /// <summary>
        /// Solves a Cubic Bézier curve point using the Bernstein method.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="control1"></param>
        /// <param name="control2"></param>
        /// <param name="end"></param>
        /// <param name="delta">Time over curve from 0 to 1</param>
        /// <returns>A point along the Bézier curve.</returns>
        public static Vector2 CubicSolve(in Vector2 start, in Vector2 control1, in Vector2 control2, in Vector2 end, float delta)
        {
            delta = Math.Clamp(delta, 0, 1);

            float omd = 1 - delta;

            return
                start * omd * omd * omd +
                control1 * 3 * omd * omd * delta +
                control2 * 3 * omd * delta * delta +
                end * delta * delta * delta;
        }

        /// <summary>
        /// Solves a Cubic Bézier curve point using the Bernstein method in 3D space.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="control1"></param>
        /// <param name="control2"></param>
        /// <param name="end"></param>
        /// <param name="delta">Time over curve from 0 to 1</param>
        /// <returns>A point along the Bézier curve.</returns>
        public static Vector3 CubicSolve(in Vector3 start, in Vector3 control1, in Vector3 control2, in Vector3 end, float delta)
        {
            delta = Math.Clamp(delta, 0, 1);

            float omd = 1 - delta;

            return
                start * omd * omd * omd +
                control1 * 3 * omd * omd * delta +
                control2 * 3 * omd * delta * delta +
                end * delta * delta * delta;
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
        /// 
        /// Note that because this function actually does the long work of multiplying and summing each part of the polynomial
        /// separately it can lead to some rather inaccurate results compared to just calling the above functions.
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
            int n = allVectors.Length - 1;
            float factN = MathX.Factorial(n);

            Vector2 sum = Vector2.Zero;

            for (int i = 0; i < allVectors.Length; i++)
            {
                int factI = MathX.Factorial(i);
                float fact = factN / (factI * MathX.Factorial(n - i));
                
                float p = MathF.Pow(omd, n - i);
                float d = MathF.Pow(delta, i);

                sum += allVectors[i] * p * d * fact;
            }

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
        /// 
        /// Note that because this function actually does the long work of multiplying and summing each part of the polynomial
        /// separately it can lead to some rather inaccurate results compared to just calling the above functions.
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
            int n = allVectors.Length - 1;
            float factN = MathX.Factorial(n);

            Vector3 sum = Vector3.Zero;

            for (int i = 0; i < allVectors.Length; i++)
            {
                int factI = MathX.Factorial(i);
                float fact = factN / (factI * MathX.Factorial(n - i));

                float p = MathF.Pow(omd, n - i);
                float d = MathF.Pow(delta, i);

                sum += allVectors[i] * p * d * fact;
            }

            return sum;
        }
    }
}
