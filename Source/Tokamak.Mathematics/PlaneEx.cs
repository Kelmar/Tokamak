using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Extensions for System.Numerics.Plane
    /// </summary>
    public static class PlaneEx
    {
        // Have to wait for C# 14 to be common to do this.

        //extension(in Plane plane)
        //{
        //    public Vector3 Location
        //    {
        //        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //        get => plane.Normal * -plane.D;
        //    }
        //}

        /// <summary>
        /// Return the location of the plane.
        /// </summary>
        /// <param name="plane">The plane who's coordinates we are interested in.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetLocation(this in Plane plane) =>
            plane.Normal * -plane.D;

        /// <summary>
        /// Get the squared distance from the plane's location to the supplied vector.
        /// </summary>
        /// <param name="plane">The plane to check.</param>
        /// <param name="location">The location we need the distance to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceToSquared(this in Plane plane, in Vector3 location)
        {
            Vector3 center = plane.GetLocation();
            return (center - location).LengthSquared();
        }

        /// <summary>
        /// Get the distance from the plane's location to the supplied vector.
        /// </summary>
        /// <param name="plane">The plane to check.</param>
        /// <param name="location">The location we need the distance to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this in Plane plane, in Vector3 location) =>
            MathF.Sqrt(plane.DistanceToSquared(location));

        /// <summary>
        /// Work horse for WhichSide()
        /// </summary>
        private static Boundary SphereWhichSide(in Plane plane, in Vector3 location, float radius)
        {
            return plane.DistanceTo(location) switch
            {
                float f when f < -radius => Boundary.Back,
                float f when f > radius => Boundary.Front,
                _ => Boundary.On
            };
        }

        /// <summary>
        /// Find out which side of the plane the given sphere falls on.
        /// </summary>
        /// <param name="plane">The plane to check</param>
        /// <param name="sphere">The sphere to check</param>
        /// <returns>
        /// Boundary.Back: Sphere is completely behind the plane.
        /// Boundary.Front: Sphere is complete infront of the plane.
        /// Boundary.On: Sphere intersects the plane in some way.
        /// </returns>
        public static Boundary WhichSide(this in Plane plane, in Sphere sphere)
            => SphereWhichSide(plane, sphere.Location, sphere.Radius);

        /// <summary>
        /// Find out which side of the plane the given point falls on.
        /// </summary>
        /// <remarks>
        /// This case is basically like the sphere test, but we're basically
        /// considering the point to be a very very small sphere who's radius is MathX.FUZ.
        /// </remarks>
        /// <param name="plane">The plane to check</param>
        /// <param name="location">The point to check</param>
        /// <returns>
        /// Boundary.Back: Point is behind the plane.
        /// Boundary.Front: Point infront of the plane.
        /// Boundary.On: Point is directly on the plane.
        /// </returns>
        public static Boundary WhichSide(this in Plane plane, in Vector3 location) =>
            SphereWhichSide(plane, location, MathX.FUZ);
    }
}
