namespace Tokamak.Graphite.PathRendering
{
    internal enum PathAction
    {
        /// <summary>
        /// For reference, not used.
        /// </summary>
        Move,

        /// <summary>
        /// Add a line
        /// </summary>
        Line,

        /// <summary>
        /// Add a quadradic Bezier curve
        /// </summary>
        BezierQuadradic,

        /// <summary>
        /// Add a cubic Bezier curve.
        /// </summary>
        BezierCubic,
    }
}
