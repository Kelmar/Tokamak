using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Graphite
{
    /// <summary>
    /// Extnesion methods for Path class.
    /// </summary>
    public static class PathEx
    {
        extension (Path path)
        {
            /// <summary>
            /// Moves the drawing cursor to a new location.
            /// </summary>
            /// <param name="x">The X coordinate to move to.</param>
            /// <param name="y">The Y coordinate to move to.</param>
            public void MoveTo(float x, float y) => path.MoveTo(new Vector2(x, y));

            /// <summary>
            /// Draw a line from the current cursor location to the new one.
            /// </summary>
            /// <param name="x">X coordinate to draw to.</param>
            /// <param name="y">Y coordinate to draw to.</param>
            public void LineTo(float x, float y) => path.LineTo(new Vector2(x, y));

            /// <summary>
            /// Draws a rectangle at the supplied coordinates.
            /// </summary>
            /// <param name="topLeft">The top/left point of the rectangle.</param>
            /// <param name="bottomRight">The bottom/right point of the rectangle.</param>
            public void Rectangle(in Vector2 topLeft, in Vector2 bottomRight)
                => path.Rectangle(RectF.FromCoordinates(topLeft, bottomRight));

            /// <summary>
            /// Draws a rounded rectangle.
            /// </summary>
            /// <remarks>
            /// If roundEdges is close enough to zero, then a regular rectangle will be drawn.
            /// </remarks>
            /// <param name="topLeft">Top left corner of the rectangle.</param>
            /// <param name="bottomRight">Bottom right corner for the rectangle.</param>
            /// <param name="roundEdges">Amount to round the corners by</param>
            public void RoundRect(in Vector2 topLeft, in Vector2 bottomRight, float roundEdges)
                => path.RoundRect(RectF.FromCoordinates(topLeft, bottomRight), roundEdges);

            /// <summary>
            /// Add a circular arc to the the path.
            /// </summary>
            /// <param name="center">The center of the circle to arc through.</param>
            /// <param name="radius">The radius of the circle to draw.</param>
            /// <param name="start">The starting angle to draw at.</param>
            /// <param name="end">The angle to end drawing at.</param>
            public void ArcTo(in Vector2 center, float radius, float start, float end)
                => path.ArcTo(center, new Vector2(radius, radius), start, end);
        }
    }
}
