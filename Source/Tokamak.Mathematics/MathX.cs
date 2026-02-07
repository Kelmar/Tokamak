using System;

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

        public static float Rad2Deg(float theta)
            => (theta / MathF.PI) * 180;

        public static double Rad2Deg(double theta)
            => (theta / Math.PI) * 180;

        public static float Deg2Rad(float theta)
            => MathF.PI * (theta / 180f);

        public static double Deg2Rad(double theta)
            => Math.PI * (theta / 180d);

        extension (double d)
        {
            /// <summary>
            /// Fuzzy almost equals compare.
            /// </summary>
            /// <param name="lhs">Left hand side</param>
            /// <param name="rhs">Right hand side</param>
            public static bool AlmostEquals(double lhs, double rhs)
            {
                double diff = Math.Abs(lhs - rhs);
                return diff <= FUZ;
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
            public static byte ToByteRange(double value) => (byte)(Math.Clamp(value, 0, 1) * Byte.MaxValue);
        }

        extension (float f)
        {
            /// <summary>
            /// Fuzzy almost equals compare.
            /// </summary>
            /// <param name="lhs">Left hand side</param>
            /// <param name="rhs">Right hand side</param>
            public static bool AlmostEquals(float lhs, float rhs)
            {
                float diff = Math.Abs(lhs - rhs);
                return diff <= FUZ;
            }

            /// <summary>
            /// Wraps a value around a given max value.
            /// </summary>
            /// <param name="v"></param>
            /// <param name="max"></param>
            public static float Wrap(float v, float max)
            {
                while (v > max)
                    v -= max;

                return v;
            }

            /// <summary>
            /// Converts a floating point value from 0 to 1 into a byte from 0 to 255.
            /// </summary>
            public static byte ToByteRange(float value) => (byte)(Math.Clamp(value, 0, 1) * Byte.MaxValue);
        }

        extension (UInt32 i)
        {
            /// <summary>
            /// Gets the most significant bit set.
            /// </summary>
            public static UInt32 MSB(UInt32 value)
            {
                value |= (value >> 1);
                value |= (value >> 2);
                value |= (value >> 4);
                value |= (value >> 8);
                value |= (value >> 16);

                return (value & ~(value >> 1));
            }

            /// <summary>
            /// Gets the next power of 2 for the given value.
            /// </summary>
            /// <remarks>
            /// This function will get the next highest power of two that will
            /// hold the supplied integer value.
            /// </remarks>
            /// <example>
            /// int i = MathX.NextPow2(640); // Returns 1024
            /// int n = MathX.NextPow2(1024); // Returns 2048
            /// </example>
            public static UInt32 NextPow2(UInt32 value) =>
                value == 0 ? 1 : MSB(value) << 1;
        }

        extension (Int32 i)
        {
            /// <summary>
            /// Get the most significant bit set.
            /// </summary>
            /// <remarks>
            /// Ignores the sign bit.
            /// </remarks>
            public static Int32 MSB(Int32 value)
            {
                // Remove sign bit
                value = value < 0 ? -value : value;

                value |= (value >> 1);
                value |= (value >> 2);
                value |= (value >> 4);
                value |= (value >> 8);

                return (value & ~(value >> 1));
            }

            /// <summary>
            /// Compute the factorial of the given positive integer <c>n</c>
            /// </summary>
            /// <param name="n">Positive integer to get the factorial of.</param>
            public static int Factorial(int n) =>
                n <= 1 ? 1 : (n * Factorial(n - 1));

            /// <summary>
            /// Gets the next power of 2 for the given value.
            /// </summary>
            /// <remarks>
            /// This function will get the next highest power of two that will
            /// hold the supplied integer value.
            /// </remarks>
            /// <example>
            /// int i = MathX.NextPow2(640); // Returns 1024
            /// int n = MathX.NextPow2(1024); // Returns 2048
            /// </example>
            public static int NextPow2(int value) =>
                value == 0 ? 1 : MSB(value) << 1;
        }
    }
}
