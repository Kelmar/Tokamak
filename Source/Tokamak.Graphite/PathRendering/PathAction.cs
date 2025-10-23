namespace Tokamak.Graphite.PathRendering
{
    internal enum PathAction
    {
        /// <summary>
        /// Used as flag to ignore first point on some shapes.
        /// </summary>
        Move,

        /// <summary>
        /// Add a line
        /// </summary>
        Line,

        /// <summary>
        /// Add a simple arc.
        /// </summary>
        Arc,

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
