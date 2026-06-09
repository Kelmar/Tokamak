using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Vector extension methods.
    /// </summary>
    public static class VectorEx
    {
        extension(in Vector2 vector)
        {
            /// <summary>
            /// Fuzzy almost equals compare.
            /// </summary>
            /// <param name="lhs">Left hand side</param>
            /// <param name="rhs">Right hand side</param>
            public static bool AlmostEquals(in Vector2 lhs, in Vector2 rhs, float fuz = MathX.FUZ)
                => MathX.AlmostEquals(lhs.X, rhs.X, fuz)
                && MathX.AlmostEquals(lhs.Y, rhs.Y, fuz);

            /// <summary>
            /// Computes the signed area of the supplied polygon.
            /// </summary>
            /// <param name="vectors">List of vectors in the polygon.</param>
            /// <remarks>
            /// The signedness of the result informs the winding of the polygon.<br/>
            /// &lt; 0 is Counterclockwise<br/>
            /// = 0 is Collinear<br/>
            /// &gt; 0 is Clockwise<br/>
            /// </remarks>
            /// <returns>The signed area of the polygon.</returns>
            public static float PolyArea(params IEnumerable<Vector2> vectors)
            {
                float area = 0;

                var points = vectors.ToList();

                int last = points.Count - 1;

                for (int i = 0; i < points.Count; last = i, ++i)
                    area += points[last].X * points[i].Y - points[i].X * points[last].Y;

                return area / 2f;
            }

            /// <summary>
            /// Compute the slope for a line that passes through the given vectors.
            /// </summary>
            /// <remarks>
            /// Given the formula of a line: y = mx + b
            /// 
            /// This function will compute the slope (m) of the line that passes through the supplied vectors.
            /// </remarks>
            /// <param name="v1">The first vector in the line.</param>
            /// <param name="v2">The second vector int he line.</param>
            /// <returns>The slope of the line.<br />
            /// 0 for horizontal lines (no slope).<br />
            /// Infinity in the event the line is vertical.
            /// </returns>
            public static float Slope(in Vector2 v1, in Vector2 v2)
            {
                Vector2 v = v2 - v1;
                return v.Y / v.X;
            }

            /// <summary>
            /// Compute the slope for a line that passes through the current and given vectors.
            /// </summary>
            /// <remarks>
            /// Given the formula of a line: <c>y = mx + b</c><br />
            /// <br />
            /// This function will compute the slope (m) of the line that passes through the supplied vectors.
            /// </remarks>
            /// <param name="v">The second vector in the line.</param>
            /// <returns>The slope of the line.<br />
            /// 0 for horizontal lines (no slope).<br />
            /// Infinity in the event the line is vertical.
            /// </returns>
            public float SlopeTo(in Vector2 v) => Vector2.Slope(vector, v);

            /// <summary>
            /// Compute the reciprocal of the slope for a line that passes through the given vectors.
            /// </summary>
            /// <remarks>
            /// Given the formula of a line: y = mx + b
            /// 
            /// This function will compute the inverse slope (1/m) of the line that passes through the supplied vectors.
            /// </remarks>
            /// <param name="v1">The first vector in the line.</param>
            /// <param name="v2">The second vector int he line.</param>
            /// <returns>The inverse slope of the line.<br />
            /// 0 for vertical lines.<br />
            /// Infinity in the event the line is horizontal (no slope)
            /// </returns>
            public static float InverseSlope(in Vector2 v1, in Vector2 v2)
            {
                Vector2 v = v2 - v1;
                return v.X / v.Y;
            }

            /// <summary>
            /// Compute the reciprocal of the slope for a line that passes through the given vectors.
            /// </summary>
            /// <remarks>
            /// Given the formula of a line: y = mx + b
            /// 
            /// This function will compute the inverse slope (1/m) of the line that passes through the supplied vectors.
            /// </remarks>
            /// <param name="v">The second vector in the line.</param>
            /// <returns>The inverse slope of the line.<br />
            /// 0 for vertical lines.<br />
            /// Infinity in the event the line is horizontal (no slope)
            /// </returns>
            public float InverseSlopeTo(in Vector2 v) => Vector2.InverseSlope(vector, v);

            /// <summary>
            /// Returns the intercept for the line that passes through the vector of the supplied slope.
            /// </summary>
            /// <remarks>
            /// Given the formula of a line: <c>y = mx + b</c><br />
            /// <br/>
            /// This function will return the intercept (b) of the line which passes through the vector with the given slope (m).
            /// </remarks>
            /// <param name="m">The slope of the line that passes through the vector.</param>
            /// <returns>
            /// The the intercept portion of a line.<br />
            /// <br />
            /// Note that in the case of a Infinity for a vertical line, this will still return Infinity as
            /// there is no intercept for vertical lines.
            /// </returns>
            public float Intercept(float m) => vector.Y - (vector.X * m);

            /// <summary>
            /// Gets the distance from the current vector to the other vector.
            /// </summary>
            public float DistanceTo(in Vector2 other) => (vector - other).Length();

            /// <summary>
            /// Computes a line that is perpendicular to the given vector.
            /// </summary>
            /// <param name="v">The starting vector.</param>
            /// <returns>A vector that is perpendicular to the supplied vector.</returns>
            public Vector2 LineNormal() => new(-vector.Y, vector.X);

            /// <summary>
            /// Performs Hermite interpolation between two vectors based on the given weighting.
            /// </summary>
            /// <param name="value1">The first vector</param>
            /// <param name="value2">The second vector</param>
            /// <param name="amount">A value between 0 and 1 that indicates the weight of value2.</param>
            /// <returns>The interpolated vector.</returns>
            public static Vector2 SmoothStep(Vector2 value1, Vector2 value2, float amount)
                => SmoothStep(value1, value2, Vector2.Create(amount));

            /// <summary>
            /// Performs Hermite interpolation between two vectors based on the given weighting.
            /// </summary>
            /// <param name="value1">The first vector</param>
            /// <param name="value2">The second vector</param>
            /// <param name="amount">A vector between 0 and 1 that indicates the weight of value2.</param>
            /// <returns>The interpolated vector.</returns>
            public static Vector2 SmoothStep(Vector2 value1, Vector2 value2, Vector2 amount)
            {
                return new Vector2(
                    float.SmoothStep(value1.X, value2.X, amount.X),
                    float.SmoothStep(value1.Y, value2.Y, amount.Y)
                );
            }

            /// <summary>
            /// Attempts to create a <seealso cref="Vector2"/> from a given array of floats.
            /// </summary>
            /// <param name="array">The values to try and generate a Vector2 from.</param>
            /// <param name="result">The resulting vector from the array, or <seealso cref="Vector2.Zero"/> if a vector could not be created.</param>
            /// <returns>True if the array had enough parameters to create a vector, false if not.</returns>
            public static bool TryFromArray(float[] array, out Vector2 result)
            {
                if (array.Length < 2)
                {
                    result = Vector2.Zero;
                    return false;
                }

                result = new Vector2(array[0], array[1]);
                return true;
            }

            /// <summary>
            /// Convert a <see cref="Vector2"/> to an array of floats.
            /// </summary>
            /// <returns>An array of floats representing a vector of [X, Y]</returns>
            public float[] ToArray() => [vector.X, vector.Y];

            /// <summary>
            /// Convert to a <seealso cref="Vector4"/>
            /// </summary>
            /// <remarks>
            /// The Z coordinate defaults to zero.
            /// </remarks>
            public Vector3 ToVector3() => new(vector.X, vector.Y, 0);

            /// <summary>
            /// Convert to a <seealso cref="Vector4"/>
            /// </summary>
            /// <remarks>
            /// The Z and W coordinates default to zero.
            /// </remarks>
            /// <returns></returns>
            public Vector4 ToVector4() => new(vector.X, vector.Y, 0, 0);
        }

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
            public static bool TryFromArray(float[] array, out Vector3 result)
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
            public static bool TryFromArray(float[] array, out Vector4 result)
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
        /// Convert float array to a Vector2.
        /// </summary>
        /// <remarks>
        /// If the array is not of sufficient length, then the vector will be initialized with zeros in the left over places.
        /// 
        /// If you wish to validate that the array is sufficient length during conversion use the <seealso cref="TryFromArray(float[], out Vector2)"/>
        /// </remarks>
        public static Vector2 ToVector2(this float[] a)
        {
            return new Vector2(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0);
        }

        /// <summary>
        /// Convert a float array to a Vector3.
        /// </summary>
        /// <remarks>
        /// If the array is not of sufficient length, then the vector will be initialized with zeros in the left over places.
        /// 
        /// If you wish to validate that the array is sufficient length during conversion use the <seealso cref="TryFromArray(float[], out Vector3)"/>
        /// </remarks>
        public static Vector3 ToVector3(this float[] a)
        {
            return new Vector3(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0);
        }

        /// <summary>
        /// Convert a float array to a Vector4.
        /// </summary>
        /// <remarks>
        /// If the array is not of sufficient length, then the vector will be initialized with zeros in the left over places.
        /// 
        /// If you wish to validate that the array is sufficient length during conversion use the <seealso cref="TryFromArray(float[], out Vector4)"/>
        /// </remarks>
        public static Vector4 ToVector4(this float[] a)
        {
            return new Vector4(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0, a.Length > 3 ? a[3] : 0);
        }
    }
}
