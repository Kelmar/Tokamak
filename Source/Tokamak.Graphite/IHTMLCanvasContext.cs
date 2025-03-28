using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Graphite
{
    /// <summary>
    /// Here for reference, will remove later.
    /// </summary>
    internal interface IHTMLCanvasContext
    {
        int Direction { get; set; }

        int FillStyle { get; set; }

        int Filter { get; set; }

        int Font { get; set; }

        int FontKerning { get; set; }

        int FontStretch { get; set; }

        int FontVariantCaps { get; set; }

        int GlobalAlpha { get; set; }

        int ImageSmothingEnable { get; set; }

        int ImageSomthingQuality { get; set; }

        int LetterSpacing { get; set; }

        int LineCap { get; set; }

        int LineDashOffset { get; set; }

        int LineJoin { get; set; }

        int LineWidth { get; set; }

        int MiterLimit { get; set; }

        int ShadowColor { get; set; }

        int ShadowOffsetX { get; set; }

        int ShadowOffsetY { get; set; }

        int StrokeStyle { get; set; }

        int TextAlign { get; set; }

        int TextBaseLine { get; set; }

        int TextRendering { get; set; }

        int WordSpacing { get; set; }


        #region Canvas Transforms

        void ResetTransform();

        void Rotate();

        void Transform();

        void Translate();

        void SetTransform();

        void Scale();

        #endregion



        #region Basic Path Manipulation

        void BeginPath();

        void ClosePath();

        void MoveTo();

        void LineTo();

        #endregion


        #region Addtional Path Manipulation

        void Arc(float x, float y, float startAngle, float endAngle, bool counterClockwise = false);

        void ArcTo(float x1, float y1, float x2, float y2, float radius);

        void BezierCurveTo();

        void Ellipse();

        void Rect();

        void RoundRect();

        void QuadraticCurveTo();

        #endregion





        // Haven't clasified these yet.



        void ClearRect();

        void Clip();

        void CreateConicGradient();

        void CreateImageData();

        void CreateLinearGradient();

        void CreatePatern();

        void CreateRadialGradient();

        void DrawFocusIfNeeded();

        void DrawImage();

        void Fill();

        void FillRect();

        void FillText();

        void GetContextAttributes();

        void GetImageData();

        void GetLineDash();

        void GetTransform();

        void IsContextLost();

        void IsPointInPath();

        void MeasureText();

        void PutImageData();

        void Reset();

        void Restore();

        void Save();

        void SetLineDash();


        // Actuall pushes the paths out to the context?
        void Stroke();

        void StrokeRect();

        void StrokeText();
    }
}
