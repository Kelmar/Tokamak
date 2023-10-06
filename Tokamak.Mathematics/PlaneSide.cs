namespace Tokamak.Mathematics
{

    /// <summary>
    /// Side of which a point falls on a plane.
    /// </summary>
    public enum PlaneSide
    {
        /// <summary>
        /// Point is on the backface side of the plane.
        /// </summary>
        Back = -1,

        /// <summary>
        /// Point is directly on the plane.
        /// </summary>
        On = 0,

        /// <summary>
        /// Point is on the front face of the plane.
        /// </summary>
        Front = 1
    }
}