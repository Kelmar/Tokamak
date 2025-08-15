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
        /// Linearly interpolate between two vectors.
        /// </summary>
        /// <param name="delta">Distance to interpolate by</param>
        /// <param name="v1">Starting value</param>
        /// <param name="v2">Ending value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 Lerp(float delta, in Vector2 v1, in Vector2 v2)
        {
            delta = Math.Clamp(delta, 0, 1);
            return (v1 * (1 - delta)) + (v2 * delta);
        }

        /// <summary>
        /// Linearly interpolate between two vectors.
        /// </summary>
        /// <param name="delta">Distance to interpolate by</param>
        /// <param name="v1">Starting value</param>
        /// <param name="v2">Ending value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3 Lerp(float delta, in Vector3 v1, in Vector3 v2)
        {
            delta = Math.Clamp(delta, 0, 1);
            return (v1 * (1 - delta)) + (v2 * delta);
        }

        /// <summary>
        /// Linearly interpolate between two vectors.
        /// </summary>
        /// <param name="delta">Distance to interpolate by</param>
        /// <param name="v1">Starting value</param>
        /// <param name="v2">Ending value</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector4 Lerp(float delta, in Vector4 v1, in Vector4 v2)
        {
            delta = Math.Clamp(delta, 0, 1);
            return (v1 * (1 - delta)) + (v2 * delta);
        }

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
