using System;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Contains various math functions.
    /// </summary>
    public static class MathX
    {
        /// <summary>
        /// Fuzzy value for almost equal tests.
        /// </summary>
        public const float FUZ = 0.000001f;

        /// <summary>
        /// Convert some angle theta from radians to degrees.
        /// </summary>
        /// <param name="theta">The angle in radians to convert to degrees.</param>
        /// <returns>The angle in degrees.</returns>
        public static float Rad2Deg(float theta)
            => (theta / MathF.PI) * 180;

        /// <summary>
        /// Convert some angle theta from radians to degrees.
        /// </summary>
        /// <param name="theta">The angle in radians to convert to degrees.</param>
        /// <returns>The angle in degrees.</returns>
        public static double Rad2Deg(double theta)
            => (theta / Math.PI) * 180;

        /// <summary>
        /// Convert some angle theta from degrees to radians.
        /// </summary>
        /// <param name="theta">The angle in degrees to convert to radians.</param>
        /// <returns>The angle in radians.</returns>
        public static float Deg2Rad(float theta)
            => MathF.PI * (theta / 180f);

        /// <summary>
        /// Convert some angle theta from degrees to radians.
        /// </summary>
        /// <param name="theta">The angle in degrees to convert to radians.</param>
        /// <returns>The angle in radians.</returns>
        public static double Deg2Rad(double theta)
            => Math.PI * (theta / 180d);

        extension (double d)
        {
            /// <summary>
            /// Fuzzy almost equals compare.
            /// </summary>
            /// <param name="lhs">Left hand side</param>
            /// <param name="rhs">Right hand side</param>
            public static bool AlmostEquals(double lhs, double rhs, double fuz = FUZ)
            {
                double diff = Math.Abs(lhs - rhs);
                return diff <= fuz;
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
            /// Performs Hermite interpolation between two values based on the given weighting.
            /// </summary>
            /// <param name="value1">The first value</param>
            /// <param name="value2">The second value</param>
            /// <param name="amount">A value between 0 and 1 that indicates the weight of value2.</param>
            /// <returns>The interpolated value.</returns>
            public static double SmoothStep(double value1, double value2, double amount)
            {
                amount = Math.Clamp((amount - value1) / (value2 - value1), 0, 1);
                return amount * amount * (3 - (2 * amount));
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
            public static bool AlmostEquals(float lhs, float rhs, float fuz = FUZ)
            {
                float diff = Math.Abs(lhs - rhs);
                return diff <= fuz;
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
            /// Performs Hermite interpolation between two values based on the given weighting.
            /// </summary>
            /// <param name="value1">The first value</param>
            /// <param name="value2">The second value</param>
            /// <param name="amount">A value between 0 and 1 that indicates the weight of value2.</param>
            /// <returns>The interpolated value.</returns>
            public static float SmoothStep(float value1, float value2, float amount)
            {
                amount = Math.Clamp((amount - value1) / (value2 - value1), 0, 1);
                return amount * amount * (3 - (2 * amount));
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
