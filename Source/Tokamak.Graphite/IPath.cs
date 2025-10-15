using System.Numerics;

namespace Tokamak.Graphite
{
    public interface IPath
    {
        void MoveTo(in Vector2 v);

        void LineTo(in Vector2 v);

        void BezierQuadradicCurveTo(in Vector2 control, in Vector2 end);

        void BezierCubicCurveTo(in Vector2 control1, in Vector2 control2, in Vector2 end);

        void Close();
    }
}
