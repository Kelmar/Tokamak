using System;
using System.Numerics;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Contains various math functions.
    /// </summary>
    public static class MathX
    {
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
        /// <param name="min">The mininum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        public static int Clamp(int v, int min, int max) => Math.Max(Math.Min(v, max), min);

        /// <summary>
        /// Clamps a value to a given range
        /// </summary>
        /// <param name="v">Value to clamp</param>
        /// <param name="min">The mininum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        public static float ClampF(float v, float min, float max) => MathF.Max(MathF.Min(v, max), min);

        /// <summary>
        /// Clamps a value to a given range
        /// </summary>
        /// <param name="v">Value to clamp</param>
        /// <param name="min">The mininum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
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

        public static Vector2 ToVector2(this float[] a)
        {
            return new Vector2(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0);
        }

        public static Vector3 ToVector3(this float[] a)
        {
            return new Vector3(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0);
        }

        public static Vector4 ToVector4(this float[] a)
        {
            return new Vector4(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0, a.Length > 3 ? a[3] : 0);
        }

        /// <summary>
        /// Converts a floating point value from 0 to 1 into a byte from 0 to 255
        /// </summary>
        public static byte ToByteRange(this float v) => (byte)(ClampF(v, 0, 1) * Byte.MaxValue);

        /// <summary>
        /// Converts a floating point value from 0 to 1 into a byte from 0 to 255
        /// </summary>
        public static byte ToByteRange(this double v) => (byte)(Clamp(v, 0, 1) * Byte.MaxValue);

        /// <summary>
        /// Returns a vector with a unit length.
        /// </summary>
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
        public static float DegToRadF(float d) => d / 180 * MathF.PI;

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        public static double DegToRad(double d) => d / 180 * Math.PI;

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        public static float RadToDegF(float r) => r / MathF.PI * 180;

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        public static double RadToDeg(double r) => r / MathF.PI * 180;

        /// <summary>
        /// Gets the next power of 2 for the given value.
        /// </summary>
        public static int NextPow2(int value)
        {
            int v = 2;

            while (v < value)
                v <<= 1;

            return v;
        }
    }
}
