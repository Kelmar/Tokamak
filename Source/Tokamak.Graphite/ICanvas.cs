using System;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Graphite
{
    public interface ICanvas
    {
        /// <summary>
        /// Current transformation matrix of the 2D drawing operations.
        /// </summary>
        Matrix3x2 Transform { get; set; }

        /// <summary>
        /// Gets or sets the current color of teh drawing pen.
        /// </summary>
        Color StrokeColor { get; set; }

        /// <summary>
        /// Gets or sets the width of the current drawing pen.
        /// </summary>
        float StrokeWidth { get; set; }

        /// <summary>
        /// Gets or sets the color which is used to fill the path.
        /// </summary>
        Color FillColor { get; set; }

        #region Basic Path Manipulation

        void BeginPath();

        void MoveTo(in Vector2 v);

        void LineTo(in Vector2 v);

        void ClosePath();

        #endregion Basic Path Manipulation

        #region Rendering Functions

        /// <summary>
        /// Draw's the current path with the current pen.
        /// </summary>
        /// <remarks>
        /// If the path contains one or fewer points; OR if the pen
        /// is empty, this operation does nothing.
        /// </remarks>
        void Stroke();

        /// <summary>
        /// Fills the current path with the current brush.
        /// </summary>
        void Fill();

        #endregion Rendering functions
    }
}
