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
        public static Vector4 Lerp(float delta, in Vector4 v1, in Vector4 v2)
        {
            delta = ClampF(delta, 0, 1);
            return (v1 * (1 - delta)) + (v2 * delta);
        }

        /// <summary>
        /// Clamps a value to a given range
        /// </summary>
        /// <param name="v">Value to clamp</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int v, int min, int max) => Math.Max(Math.Min(v, max), min);

        /// <summary>
        /// Clamps a value to a given range
        /// </summary>
        /// <param name="v">Value to clamp</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampF(float v, float min, float max) => MathF.Max(MathF.Min(v, max), min);

        /// <summary>
        /// Clamps a value to a given range
        /// </summary>
        /// <param name="v">Value to clamp</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Convert float array to a Vector2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this float[] a)
        {
            return new Vector2(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0);
        }

        /// <summary>
        /// Convert a float array to a Vector3
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this float[] a)
        {
            return new Vector3(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0);
        }

        /// <summary>
        /// Convert a float array to a Vector4
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this float[] a)
        {
            return new Vector4(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0, a.Length > 3 ? a[3] : 0);
        }

        /// <summary>
        /// Converts a floating point value from 0 to 1 into a byte from 0 to 255
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByteRange(this float v) => (byte)(ClampF(v, 0, 1) * Byte.MaxValue);

        /// <summary>
        /// Converts a floating point value from 0 to 1 into a byte from 0 to 255
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToByteRange(this double v) => (byte)(Clamp(v, 0, 1) * Byte.MaxValue);

        /// <summary>
        /// Convert gamma color value to linear color value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GammaToLinear(byte b, double gamma) => (255d * Math.Pow(b / 255d, gamma));

        /// <summary>
        /// Convert linear color value to gamma color value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte LinearToGamma(double d, double gamma) => (byte)Math.Round(255 * Math.Pow(d / 255d, 1 / gamma));

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
        /// Convert degrees to radians
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DegToRadF(float d) => d / 180 * MathF.PI;

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DegToRad(double d) => d / 180 * Math.PI;

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadToDegF(float r) => r / MathF.PI * 180;

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double RadToDeg(double r) => r / MathF.PI * 180;

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
