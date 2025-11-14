using System.Numerics;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Vector extension methods.
    /// </summary>
    public static class VectorEx
    {
        extension (in Vector2 vector)
        {
            /// <summary>
            /// Gets the distance from the current vector to the other vector.
            /// </summary>
            public float DistanceTo(in Vector2 other) => (vector - other).Length();

            /// <summary>
            /// Computes a line that is perpendicular to the given vector.
            /// </summary>
            /// <param name="v">The starting vector.</param>
            /// <returns>A vector that is perpendicular to the supplied vector.</returns>
            public Vector2 LineNormal() => new Vector2(-vector.Y, vector.X);

            /// <summary>
            /// Returns the cross product of two 2D vectors.
            /// </summary>
            /// <remarks>
            /// Technically there is no cross product for 2D vectors.  We get
            /// around this by treating them as 3D vectors with a zero Z part.
            /// 
            /// This ultimately reduces down to a Vector3D who's X and Y will 
            /// always be zero as a result, with the Z component being the only
            /// interesting result.
            /// </remarks>
            public static float Cross(in Vector2 v1, in Vector2 v2) => v1.X * v2.Y - v2.X * v1.Y;

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
        }

        extension (in Vector3 vector)
        {
            /// <summary>
            /// Gets the distance from the current vector to the other vector.
            /// </summary>
            public float DistanceTo(in Vector3 other) => (vector - other).Length();

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
        }

        extension (in Vector4 vector)
        {
            /// <summary>
            /// Gets the distance from the current vector to the other vector.
            /// </summary>
            public float DistanceTo(in Vector4 other) => (vector - other).Length();

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
        }

        /// <summary>
        /// Convert float array to a Vector2.
        /// </summary>
        /// <remarks>
        /// If the array is not of sufficent length, then the vector will be initialized with zeros in the left over places.
        /// 
        /// If you wish to validate that the array is sufficent length during conversion use the <seealso cref="TryFromArray(float[], out Vector2)"/>
        /// </remarks>
        public static Vector2 ToVector2(this float[] a)
        {
            return new Vector2(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0);
        }

        /// <summary>
        /// Convert a float array to a Vector3.
        /// </summary>
        /// <remarks>
        /// If the array is not of sufficent length, then the vector will be initialized with zeros in the left over places.
        /// 
        /// If you wish to validate that the array is sufficent length during conversion use the <seealso cref="TryFromArray(float[], out Vector3)"/>
        /// </remarks>
        public static Vector3 ToVector3(this float[] a)
        {
            return new Vector3(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0);
        }

        /// <summary>
        /// Convert a float array to a Vector4.
        /// </summary>
        /// <remarks>
        /// If the array is not of sufficent length, then the vector will be initialized with zeros in the left over places.
        /// 
        /// If you wish to validate that the array is sufficent length during conversion use the <seealso cref="TryFromArray(float[], out Vector4)"/>
        /// </remarks>
        public static Vector4 ToVector4(this float[] a)
        {
            return new Vector4(a.Length > 0 ? a[0] : 0, a.Length > 1 ? a[1] : 0, a.Length > 2 ? a[2] : 0, a.Length > 3 ? a[3] : 0);
        }
    }
}
