namespace Tokamak.Mathematics
{

    /// <summary>
    /// Side of which a point falls on an object.
    /// </summary>
    public enum Boundary
    {
        /// <summary>
        /// Point is behind the plane.
        /// </summary>
        Back = -1,

        /// <summary>
        /// Point is on the inside of a volume.
        /// </summary>
        Inside = -1,

        /// <summary>
        /// Point is directly on the bounds of the object.
        /// </summary>
        On = 0,

        /// <summary>
        /// Point is in front of the plane.
        /// </summary>
        Front = 1,

        /// <summary>
        /// Point is outside of the volume.
        /// </summary>
        Outside = 1
    }
}