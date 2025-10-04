using System;
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
        /// Fuzzy almost equals compare.
        /// </summary>
        /// <param name="lhs">Left hand side</param>
        /// <param name="rhs">Right hand side</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool AlmostEquals(float lhs, float rhs)
        {
            float diff = Math.Abs(lhs - rhs);
            return diff <= FUZ;
        }

        /// <summary>
        /// Linearly interpolate between two values.
        /// </summary>
        /// <param name="v1">Starting value</param>
        /// <param name="v2">Ending value</param>
        /// <param name="delta">Distance to interpolate by</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double Lerp(double v1, double v2, double delta)
        {
            delta = Math.Clamp(delta, 0, 1);
            return (v1 * (1 - delta)) + (v2 * delta);
        }

        /// <summary>
        /// Linearly interpolate between two values.
        /// </summary>
        /// <param name="v1">Starting value</param>
        /// <param name="v2">Ending value</param>
        /// <param name="delta">Distance to interpolate by</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float LerpF(float v1, float v2, float delta)
        {
            delta = Math.Clamp(delta, 0, 1);
            return (v1 * (1 - delta)) + (v2 * delta);
        }

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
        /// Converts a floating point value from 0 to 1 into a byte from 0 to 255.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByteRange(this float v) => (byte)(Math.Clamp(v, 0, 1) * Byte.MaxValue);

        /// <summary>
        /// Converts a floating point value from 0 to 1 into a byte from 0 to 255.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByteRange(this double v) => (byte)(Math.Clamp(v, 0, 1) * Byte.MaxValue);

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
