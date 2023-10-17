using System.Numerics;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Mathmatical representation of a 3D plane.
    /// </summary>
    /// <remarks>
    /// Mathematically we define a plane as having a normalized vector 
    /// centered at the origin point, and then we scale long that normal
    /// to set it's actual position.
    /// </remarks>
    public class Plane
    {
        public Vector3 Normal { get; set; } = Vector3.Zero;

        public float Distance { get; set; } = 0;

        public Vector3 Position => Normal * -Distance;
    }
}