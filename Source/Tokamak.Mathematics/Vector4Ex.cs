using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Tokamak.Mathematics
{
    public static class Vector4Ex
    {
        extension(in Vector4 vector)
        {
            /// <summary>
            /// Fuzzy almost equals compare.
            /// </summary>
            /// <param name="lhs">Left hand side</param>
            /// <param name="rhs">Right hand side</param>
            public static bool AlmostEquals(in Vector4 lhs, in Vector4 rhs, float fuz = MathX.FUZ)
                => MathX.AlmostEquals(lhs.X, rhs.X, fuz)
                && MathX.AlmostEquals(lhs.Y, rhs.Y, fuz)
                && MathX.AlmostEquals(lhs.Z, rhs.Z, fuz)
                && MathX.AlmostEquals(lhs.W, rhs.W, fuz);

            /// <summary>
            /// Gets the distance from the current vector to the other vector.
            /// </summary>
            public float DistanceTo(in Vector4 other) => (vector - other).Length();

            /// <summary>
            /// Performs Hermite interpolation between two vectors based on the given weighting.
            /// </summary>
            /// <param name="value1">The first vector</param>
            /// <param name="value2">The second vector</param>
            /// <param name="amount">A value between 0 and 1 that indicates the weight of value2.</param>
            /// <returns>The interpolated vector.</returns>
            public static Vector4 SmoothStep(Vector4 value1, Vector4 value2, float amount)
                => SmoothStep(value1, value2, Vector4.Create(amount));

            /// <summary>
            /// Performs Hermite interpolation between two vectors based on the given weighting.
            /// </summary>
            /// <param name="value1">The first vector</param>
            /// <param name="value2">The second vector</param>
            /// <param name="amount">A vector between 0 and 1 that indicates the weight of value2.</param>
            /// <returns>The interpolated vector.</returns>
            public static Vector4 SmoothStep(Vector4 value1, Vector4 value2, Vector4 amount)
            {
                return new Vector4(
                    float.SmoothStep(value1.X, value2.X, amount.X),
                    float.SmoothStep(value1.Y, value2.Y, amount.Y),
                    float.SmoothStep(value1.Z, value2.Z, amount.Z),
                    float.SmoothStep(value1.W, value2.W, amount.W)
                );
            }

            /// <summary>
            /// Attempts to create a <seealso cref="Vector4"/> from a given array of floats.
            /// </summary>
            /// <param name="array">The values to try and generate a Vector2 from.</param>
            /// <param name="result">The resulting vector from the array, or <seealso cref="Vector4.Zero"/> if a vector could not be created.</param>
            /// <returns>True if the array had enough parameters to create a vector, false if not.</returns>
            public static bool TryFromArray(in ReadOnlyMemory<float> array, out Vector4 result)
                => TryFromArray(array.Span, out result);

            /// <summary>
            /// Attempts to create a <seealso cref="Vector4"/> from a given array of floats.
            /// </summary>
            /// <param name="array">The values to try and generate a Vector2 from.</param>
            /// <param name="result">The resulting vector from the array, or <seealso cref="Vector4.Zero"/> if a vector could not be created.</param>
            /// <returns>True if the array had enough parameters to create a vector, false if not.</returns>
            public static bool TryFromArray(in ReadOnlySpan<float> array, out Vector4 result)
            {
                if (array.Length < 4)
                {
                    result = Vector4.Zero;
                    return false;
                }

                result = new Vector4(array[0], array[1], array[2], array[3]);
                return true;
            }

            /// <summary>
            /// Convert a enumeration of floats to a Vector4.
            /// </summary>
            /// <remarks>
            /// If the array is not of sufficient length, then the vector will be initialized with zeros in the left over places.
            /// 
            /// If you wish to validate that the array is sufficient length during conversion use the <seealso cref="TryFromArray(float[], out Vector4)"/>
            /// </remarks>
            public static Vector4 FromEnumerable(IEnumerable<float> floats)
            {
                var array = floats.ToArray();
                return ToVector4(array);
            }

            /// <summary>
            /// Convert a <see cref="Vector2"/> to an array of floats.
            /// </summary>
            /// <returns>An array of floats representing a vector of [X, Y, Z, W]</returns>
            public float[] ToArray() => [vector.X, vector.Y, vector.Z, vector.W];

            /// <summary>
            /// Convert to a <seealso cref="Vector2"/>
            /// </summary>
            /// <remarks>
            /// The Z and W coordinates are dropped.
            /// </remarks>
            /// <returns></returns>
            public Vector2 ToVector2() => new(vector.X, vector.Y);

            /// <summary>
            /// Convert to a <seealso cref="Vector2"/>
            /// </summary>
            /// <remarks>
            /// The W coordinate is dropped.
            /// </remarks>
            /// <returns></returns>
            public Vector3 ToVector3() => new(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Convert a ReadOnlyMemory of floats to a Vector4.
        /// </summary>
        /// <remarks>
        /// If the array is not of sufficient length, then the vector will be initialized with zeros in the left over places.
        /// 
        /// If you wish to validate that the array is sufficient length during conversion use the <seealso cref="TryFromArray(float[], out Vector4)"/>
        /// </remarks>
        public static Vector4 ToVector4(this in ReadOnlyMemory<float> a)
            => ToVector4(a.Span);

        /// <summary>
        /// Convert a ReadOnlySpan of floats to a Vector4.
        /// </summary>
        /// <remarks>
        /// If the array is not of sufficient length, then the vector will be initialized with zeros in the left over places.
        /// 
        /// If you wish to validate that the array is sufficient length during conversion use the <seealso cref="TryFromArray(float[], out Vector4)"/>
        /// </remarks>
        public static Vector4 ToVector4(this in ReadOnlySpan<float> a)
            => new Vector4(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0, a.Length > 3 ? a[3] : 0);
    }
}
