using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Contains various math functions.
    /// </summary>
    public static class MathX
    {
        /// <summary>
        /// Fuzzy value for roughly equal tests.
        /// </summary>
        public const float FUZ = 0.000001f;

        /// <summary>
        /// Linearly interpolate between two values.
        /// </summary>
        /// <param name="delta">Distance to interpolate by</param>
        /// <param name="v1">Starting value</param>
        /// <param name="v2">Ending value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double Lerp(double delta, double v1, double v2)
        {
            delta = Clamp(delta, 0, 1);
            return (v1 * (1 - delta)) + (v2 * delta);
        }

        /// <summary>
        /// Linearly interpolate between two values.
        /// </summary>
        /// <param name="delta">Distance to interpolate by</param>
        /// <param name="v1">Starting value</param>
        /// <param name="v2">Ending value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float LerpF(float delta, float v1, float v2)
        {
            delta = ClampF(delta, 0, 1);
            return (v1 * (1 - delta)) + (v2 * delta);
        }

        /// <summary>
        /// Linearly interpolate between two vectors.
        /// </summary>
        /// <param name="delta">Distance to interpolate by</param>
        /// <param name="v1">Starting value</param>
        /// <param name="v2">Ending value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 Lerp(float delta, in Vector2 v1, in Vector2 v2)
        {
            delta = ClampF(delta, 0, 1);
            return (v1 * (1 - delta)) + (v2 * delta);
        }

        /// <summary>
        /// Linearly interpolate between two vectors.
        /// </summary>
        /// <param name="delta">Distance to interpolate by</param>
        /// <param name="v1">Starting value</param>
        /// <param name="v2">Ending value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3 Lerp(float delta, in Vector3 v1, in Vector3 v2)
        {
            delta = ClampF(delta, 0, 1);
            return (v1 * (1 - delta)) + (v2 * delta);
        }

        /// <summary>
        /// Linearly interpolate between two vectors.
        /// </summary>
        /// <param name="delta">Distance to interpolate by</param>
        /// <param name="v1">Starting value</param>
        /// <param name="v2">Ending value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector4 Lerp(float delta, in Vector4 v1, in Vector4 v2)
        {
            delta = ClampF(delta, 0, 1);
            return (v1 * (1 - delta)) + (v2 * delta);
        }

        /// <summary>
        /// Solves a Quadradic Bézier curve point using the Bernstein method.
        /// </summary>
        /// <remarks>
        /// See notes on <seealso cref="CubicBezierSolve(in Vector2, in Vector2, in Vector2, in Vector2, float)"/>
        /// for more details.
        /// </remarks>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="delta"Time over curve from 0 to 1></param>
        /// <returns>A point along the Bézier curve.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 QuadBezierSolve(in Vector2 v1, in Vector2 v2, in Vector2 v3, float delta)
        {
            delta = ClampF(delta, 0, 1);

            float sqDelta = delta * delta;

            return
                v1 * (delta - 1) * (delta - 1) +
                v2 * (2 * delta - 2 * sqDelta) +
                v3 * sqDelta;
        }

        /// <summary>
        /// Solves a Cubic Bézier curve point using the Bernstein method.
        /// </summary>
        /// <remarks>
        /// References:
        /// https://en.wikipedia.org/wiki/B%C3%A9zier_curve
        /// https://www.youtube.com/watch?v=aVwxzDHniEw
        /// 
        /// In summary the Bernstein method is what you get if you carray out the
        /// DeCasteljau version of using lerp and reduce down to a large polynomial.
        /// 
        /// The DeCasteljau version is if you performed lerps on each of the line
        /// segments recursively.
        /// </remarks>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="delta">Time over curve from 0 to 1</param>
        /// <returns>A point along the Bézier curve.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 CubicBezierSolve(in Vector2 v1, in Vector2 v2, in Vector2 v3, in Vector2 v4, float delta)
        {
            delta = ClampF(delta, 0, 1);

            float cubeDelta = delta * delta * delta;
            float sqDelta = delta * delta;

            return
                v1 * (-cubeDelta + 3 * sqDelta - 3 * delta + 1) +
                v2 * (3 * cubeDelta - 6 * sqDelta + 3 * delta) +
                v3 * (-3 * cubeDelta + 3 * sqDelta) +
                v4 * (cubeDelta);
        }

        /// <summary>
        /// Clamps a value to a given range.
        /// </summary>
        /// <param name="v">Value to clamp</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int Clamp(int v, int min, int max) => Math.Max(Math.Min(v, max), min);

        /// <summary>
        /// Clamps a value to a given range.
        /// </summary>
        /// <param name="v">Value to clamp</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float ClampF(float v, float min, float max) => MathF.Max(MathF.Min(v, max), min);

        /// <summary>
        /// Clamps a value to a given range.
        /// </summary>
        /// <param name="v">Value to clamp</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double Clamp(double v, double min, double max) => Math.Max(Math.Min(v, max), min);

        /// <summary>
        /// Wraps a value around a given max value.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="max"></param>
        public static float WrapF(float v, float max)
        {
            while (v > max)
                v -= max;

            return v;
        }

        /// <summary>
        /// Wraps a value around a given max value.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="max"></param>
        public static double Wrap(double v, double max)
        {
            while (v > max)
                v -= max;

            return v;
        }

        /// <summary>
        /// Convert float array to a Vector2.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this float[] a)
        {
            return new Vector2(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0);
        }

        /// <summary>
        /// Convert a float array to a Vector3.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this float[] a)
        {
            return new Vector3(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0);
        }

        /// <summary>
        /// Convert a float array to a Vector4.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this float[] a)
        {
            return new Vector4(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0, a.Length > 3 ? a[3] : 0);
        }

        /// <summary>
        /// Gets the distance from the current vector to the other vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this in Vector2 v1, in Vector2 v2)
        {
            return (v1 - v2).Length();
        }

        /// <summary>
        /// Gets the distance from the current vector to the other vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this in Vector3 v1, in Vector3 v2)
        {
            return (v1 - v2).Length();
        }

        /// <summary>
        /// Gets the distance from the current vector to the other vector.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this in Vector4 v1, in Vector4 v2)
        {
            return (v1 - v2).Length();
        }

        /// <summary>
        /// Converts a floating point value from 0 to 1 into a byte from 0 to 255.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByteRange(this float v) => (byte)(ClampF(v, 0, 1) * Byte.MaxValue);

        /// <summary>
        /// Converts a floating point value from 0 to 1 into a byte from 0 to 255.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByteRange(this double v) => (byte)(Clamp(v, 0, 1) * Byte.MaxValue);

        /// <summary>
        /// Returns a vector with a unit length.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Normalize(this in Vector2 v)
        {
            float len = v.Length();

            if (len == 0)
                return Vector2.Zero;

            return v / len;
        }

        /// <summary>
        /// Returns a vector with a unit length.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Normalize(this in Vector3 v)
        {
            float len = v.Length();

            if (len == 0)
                return Vector3.Zero;

            return v / len;
        }

        /// <summary>
        /// Returns a vector with a unit length.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Normalize(this in Vector4 v)
        {
            float len = v.Length();

            if (len == 0)
                return Vector4.Zero;

            return v / len;
        }

        /// <summary>
        /// Returns the cross product of two 2D vectors.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cross(in Vector2 v1, in Vector2 v2) => v1.X * v2.Y - v2.X * v1.Y;

        /// <summary>
        /// Create a 3x2 matrix that skews along the X axis.
        /// </summary>
        public static Matrix3x2 CreateSkewX(float a)
        {
            Matrix3x2 rval = Matrix3x2.Identity;
            rval.M21 = MathF.Tan(a);

            return rval;
        }

        /// <summary>
        /// Create a 3x2 matrix that skews along the Y axis.
        /// </summary>
        public static Matrix3x2 CreateSkewY(float a)
        {
            Matrix3x2 rval = Matrix3x2.Identity;
            rval.M12 = MathF.Tan(a);

            return rval;
        }

        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DegToRadF(float d) => d / 180f * MathF.PI;

        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DegToRad(double d) => d / 180d * Math.PI;

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadToDegF(float r) => r / MathF.PI * 180f;

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double RadToDeg(double r) => r / MathF.PI * 180d;

        /// <summary>
        /// Gets the most significant bit set.
        /// </summary>
        public static UInt32 MSB(UInt32 i)
        {
            i |= (i >> 1);
            i |= (i >> 2);
            i |= (i >> 4);
            i |= (i >> 8);
            i |= (i >> 16);

            return (i & ~(i >> 1));
        }

        /// <summary>
        /// Get the most significant bit set.
        /// </summary>
        /// <remarks>
        /// Ignores the sign bit.
        /// </remarks>
        public static Int32 MSB(Int32 i)
        {
            // Remove sign bit
            i = i < 0 ? -i : i;

            i |= (i >> 1);
            i |= (i >> 2);
            i |= (i >> 4);
            i |= (i >> 8);

            return (i & ~(i >> 1));
        }

        /// <summary>
        /// Gets the next power of 2 for the given value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NextPow2(int value) => MSB(value) << 1;
    }
}
