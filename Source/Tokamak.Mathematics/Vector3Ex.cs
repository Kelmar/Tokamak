using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Tokamak.Mathematics
{
    public static class Vector3Ex
    {
        extension(in Vector3 vector)
        {
            /// <summary>
            /// Fuzzy almost equals compare.
            /// </summary>
            /// <param name="lhs">Left hand side</param>
            /// <param name="rhs">Right hand side</param>
            public static bool AlmostEquals(in Vector3 lhs, in Vector3 rhs, float fuz = MathX.FUZ)
                => MathX.AlmostEquals(lhs.X, rhs.X, fuz)
                && MathX.AlmostEquals(lhs.Y, rhs.Y, fuz)
                && MathX.AlmostEquals(lhs.Z, rhs.Z, fuz);

            /// <summary>
            /// Gets the distance from the current vector to the other vector.
            /// </summary>
            public float DistanceTo(in Vector3 other) => (vector - other).Length();

            /// <summary>
            /// Returns the distance from the point to the surface of the given sphere.
            /// </summary>
            /// <remarks>
            /// If the value is roughly equal to zero then the point lies on the surface of the sphere.
            /// If the value is less than zero then the point is inside of the sphere.
            /// </remarks>
            /// <param name="sphere">The sphere to get the distance to.</param>
            /// <returns>The distance of the point to the surface of the sphere.</returns>
            public float DistanceTo(in Sphere sphere) => sphere.DistanceTo(vector);

            /// <summary>
            /// Performs Hermite interpolation between two vectors based on the given weighting.
            /// </summary>
            /// <param name="value1">The first vector</param>
            /// <param name="value2">The second vector</param>
            /// <param name="amount">A value between 0 and 1 that indicates the weight of value2.</param>
            /// <returns>The interpolated vector.</returns>
            public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount)
                => SmoothStep(value1, value2, Vector3.Create(amount));

            /// <summary>
            /// Performs Hermite interpolation between two vectors based on the given weighting.
            /// </summary>
            /// <param name="value1">The first vector</param>
            /// <param name="value2">The second vector</param>
            /// <param name="amount">A vector between 0 and 1 that indicates the weight of value2.</param>
            /// <returns>The interpolated vector.</returns>
            public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, Vector3 amount)
            {
                return new Vector3(
                    float.SmoothStep(value1.X, value2.X, amount.X),
                    float.SmoothStep(value1.Y, value2.Y, amount.Y),
                    float.SmoothStep(value1.Z, value2.Z, amount.Z)
                );
            }

            /// <summary>
            /// Attempts to create a <seealso cref="Vector3"/> from a given array of floats.
            /// </summary>
            /// <param name="array">The values to try and generate a Vector2 from.</param>
            /// <param name="result">The resulting vector from the array, or <seealso cref="Vector3.Zero"/> if a vector could not be created.</param>
            /// <returns>True if the array had enough parameters to create a vector, false if not.</returns>
            public static bool TryFromArray(in ReadOnlyMemory<float> array, out Vector3 result)
                => TryFromArray(array.Span, out result);

            /// <summary>
            /// Attempts to create a <seealso cref="Vector3"/> from a given array of floats.
            /// </summary>
            /// <param name="array">The values to try and generate a Vector2 from.</param>
            /// <param name="result">The resulting vector from the array, or <seealso cref="Vector3.Zero"/> if a vector could not be created.</param>
            /// <returns>True if the array had enough parameters to create a vector, false if not.</returns>
            public static bool TryFromArray(in ReadOnlySpan<float> array, out Vector3 result)
            {
                if (array.Length < 3)
                {
                    result = Vector3.Zero;
                    return false;
                }

                result = new Vector3(array[0], array[1], array[2]);
                return true;
            }

            /// <summary>
            /// Convert a enumeration of floats to a Vector3.
            /// </summary>
            /// <remarks>
            /// If the array is not of sufficient length, then the vector will be initialized with zeros in the left over places.
            /// 
            /// If you wish to validate that the array is sufficient length during conversion use the <seealso cref="TryFromArray(float[], out Vector4)"/>
            /// </remarks>
            public static Vector3 FromEnumeration(IEnumerable<float> floats)
            {
                var array = floats.ToArray();
                return ToVector3(array);
            }

            /// <summary>
            /// Convert a <see cref="Vector2"/> to an array of floats.
            /// </summary>
            /// <returns>An array of floats representing a vector of [X, Y, Z]</returns>
            public float[] ToArray() => [vector.X, vector.Y, vector.Z];


            /// <summary>
            /// Convert to a <seealso cref="Vector2"/>
            /// </summary>
            /// <remarks>
            /// The Z coordinate is dropped.
            /// </remarks>
            /// <returns></returns>
            public Vector2 ToVector2() => new(vector.X, vector.Y);

            /// <summary>
            /// Convert to a <seealso cref="Vector4"/>
            /// </summary>
            /// <remarks>
            /// The W coordinate defaults to zero.
            /// </remarks>
            /// <returns></returns>
            public Vector4 ToVector4() => new(vector.X, vector.Y, vector.Z, 0);
        }


        /// <summary>
        /// Convert a ReadOnlyMemory of floats to a Vector3.
        /// </summary>
        /// <remarks>
        /// If the array is not of sufficient length, then the vector will be initialized with zeros in the left over places.
        /// 
        /// If you wish to validate that the array is sufficient length during conversion use the <seealso cref="TryFromArray(float[], out Vector3)"/>
        /// </remarks>
        public static Vector3 ToVector3(this in ReadOnlyMemory<float> a)
            => ToVector3(a.Span);

        /// <summary>
        /// Convert a ReadOnlySpan of floats to a Vector3.
        /// </summary>
        /// <remarks>
        /// If the array is not of sufficient length, then the vector will be initialized with zeros in the left over places.
        /// 
        /// If you wish to validate that the array is sufficient length during conversion use the <seealso cref="TryFromArray(float[], out Vector3)"/>
        /// </remarks>
        public static Vector3 ToVector3(this in ReadOnlySpan<float> a)
            => new Vector3(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0);

    }
}
