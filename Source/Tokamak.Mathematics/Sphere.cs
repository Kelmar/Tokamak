﻿using System;
using System.Numerics;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Mathematical representation of a sphere.
    /// </summary>
    /// <remarks>
    /// A sphere has an origin point and a radius.
    /// </remarks>
    public struct Sphere
    {
        public Sphere()
        {
            // Default to unit sphere at center of space.
            Location = Vector3.Zero;
            Radius = 1f;
        }

        public Sphere(Vector3 location, float radius)
        {
            Location = location;
            Radius = radius;
        }

        /// <summary>
        /// Origin point of the sphere.
        /// </summary>
        public Vector3 Location { get; set; }

        /// <summary>
        /// The sphere's radius.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Check to see if other sphere overlaps with this sphere.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if the two spheres overlap with each other.</returns>
        public bool Overlaps(in Sphere other)
        {
            return Location.DistanceTo(other.Location) < (Radius + other.Radius);
        }

        /// <summary>
        /// Check to see if a point is inside the sphere or not.
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>The location of the point relative to the sphere.</returns>
        public Boundary Contains(in Vector3 point)
        {
            return Location.DistanceTo(point) switch
            {
                float f when f < -MathX.FUZ => Boundary.Inside,
                float f when f > MathX.FUZ => Boundary.Outside,
                _ => Boundary.On
            };
        }

        public bool RayTest(in Vector3 start, in Vector3 end)
        {
            var dir = (end - start).Normalize();

            Vector3 oc = start - Location;

            // The 'a' component will always be 1 because we normalized the direction.
            //float a = dir.LengthSquared();
            float b = 2 * Vector3.Dot(dir, oc);
            float c = oc.LengthSquared() - (Radius * Radius);

            //float descriminate = (b * b) - (4 * a * c);
            float descriminate = (b * b) - (4 * c);

            // If the discriminate is less than zero then the ray missed the sphere entirely.
            if (descriminate < 0)
                return false;

            float dSqrt = MathF.Sqrt(descriminate);

            /*
             * Because we have a square root, there are actually two values
             * to this solution.  Which makes sense when you consider that
             * the ray likely will pass through two points when it intersects
             * with the sphere.
             */

            //float q = ((b < 0) ? (-b - dSqrt) : (-b + dSqrt)) / 2;
            //float t0 = q / a;

            float t0 = ((b < 0) ? (-b - dSqrt) : (-b + dSqrt)) / 2;
            float t1 = c / t0;

            if (t0 > t1)
                (t0, t1) = (t1, t0); // Ensure t0 < t1

            if (t1 < 0)
            {
                // If t1 is less than zero, then the sphere is before the ray start.
                return false;
            }

            // This would return the time delta along the ray the intersection is at:

            // If t0 is less than zero the point is at t1
            //return t0 < 0 ? t1 : t0; 

            return true;
        }
    }
}
