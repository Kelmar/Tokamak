using System.Numerics;
using System.Runtime.CompilerServices;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Extensions for System.Numerics.Plane
    /// </summary>
    public static class PlaneEx
    {
        /// <summary>
        /// Get the distance from the plane's location to the supplied vector.
        /// </summary>
        /// <param name="plane">The plane to check.</param>
        /// <param name="location">The location we need the distance to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceTo(this in Plane plane, in Vector3 location) =>
            (plane.Normal.X * location.X) + (plane.Normal.Y * location.Y) + (plane.Normal.Z * location.Z) - plane.D;

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
        public static Boundary WhichSide(this in Plane plane, in Vector3 location, float radius = MathX.FUZ)
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
        public static Boundary WhichSide(this in Plane plane, in Sphere sphere) =>
            WhichSide(plane, sphere.Location, sphere.Radius);


        /// <summary>
        /// Test to see if a ray segment intersects the plane.
        /// </summary>
        /// <param name="plane">The plane that the intersection will be tested against.</param>
        /// <param name="start">The start of the ray segment</param>
        /// <param name="end">The end of the ray segment.</param>
        /// <param name="t">The resulting time delta along the ray originating from start where the intersection occurs.</param>
        /// <returns>true the ray intersects, false if not.</returns>
        public static bool RayTest(this in Plane plane, in Vector3 start, in Vector3 end, out float t)
        {
            float den = Vector3.Dot(end - start, plane.Normal);

            if (MathX.AlmostEquals(den, 0))
            {
                t = 0;
                return false;
            }

            t = -plane.DistanceTo(start) / den;
            return true;
        }
    }
}
