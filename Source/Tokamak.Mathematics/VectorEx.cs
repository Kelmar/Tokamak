using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Vector extension methods.
    /// </summary>
    public static class VectorEx
    {
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
        /// Computes a line that is perpendicular to the given vector.
        /// </summary>
        /// <param name="v">The starting vector.</param>
        /// <returns>A vector that is perpendicular to the supplied vector.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 LineNormal(this in Vector2 v) => new Vector2(-v.Y, v.X);

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cross(in Vector2 v1, in Vector2 v2) => v1.X * v2.Y - v2.X * v1.Y;
    }
}
