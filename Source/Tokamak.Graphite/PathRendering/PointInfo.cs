using System.Numerics;

namespace Tokamak.Graphite.PathRendering
{
    /// <summary>
    /// Information about our points.
    /// </summary>
    internal class PointInfo
    {
        /// <summary>
        /// Control point location on our path.
        /// </summary>
        public Vector2 Point { get; set; }

        /// <summary>
        /// The direction the path goes from the Point location.
        /// </summary>
        public Vector2 Direction { get; set; }

        /// <summary>
        /// A normalized perpendicular vector to the direction vector.
        /// </summary>
        public Vector2 Normal { get; set; }

        /// <summary>
        /// Length of this path segment.
        /// </summary>
        public float Length { get; set; }

        /// <summary>
        /// Vector giving the direction of the Miter pointing to the left of our point.
        /// </summary>
        public Vector2 Miter { get; set; }
    }
}
