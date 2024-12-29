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
            float distance = (Location - other.Location).Length();
            return distance < (Radius + other.Radius);
        }

        /// <summary>
        /// Check to see if a point is inside the sphere or not.
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>The location of the point relative to the sphere.</returns>
        public Boundary Contains(in Vector3 point)
        {
            float distance = (Location - point).Length();
            return distance switch
            {
                float f when f < -MathX.FUZ => Boundary.Inside,
                float f when f > MathX.FUZ => Boundary.Outside,
                _ => Boundary.On
            };
        }
    }
}
