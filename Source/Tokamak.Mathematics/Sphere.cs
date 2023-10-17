using System.Numerics;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Mathamatical representation of a sphere.
    /// </summary>
    /// <remarks>
    /// A sphere has an origin point and a radius.
    /// </remarks>
    public class Sphere
    {
        public Vector3 Location { get; set; }

        public float Radius { get; set; } = 1f;
    }
}
