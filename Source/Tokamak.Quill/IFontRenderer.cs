using System.Numerics;

namespace Tokamak.Quill
{
    public interface IFontRenderer
    {
        Matrix3x2 Transform { get; set; }

        void DrawLine(Vector2 v1, Vector2 v2);

        void DrawQuadradic(Vector2 v1, Vector2 v2, Vector2 v3);

        void DrawCubic(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4);
    }
}
