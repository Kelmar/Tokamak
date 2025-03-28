using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

using Tokamak.Quill;

namespace Tokamak.Graphite
{
    internal class GraphiteFontRenderer : IFontRenderer
    {
        private readonly Canvas m_owner;
        private readonly Pen m_pen;

        public GraphiteFontRenderer(Canvas owner, Pen pen)
        {
            m_owner = owner;
            m_pen = pen;
        }

        public Matrix3x2 Transform
        {
            get => Matrix3x2.Identity;
            set { }
        }

        public void DrawLine(Vector2 v1, Vector2 v2)
        {
        }

        public void DrawCubic(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
        {
        }

        public void DrawQuadradic(Vector2 v1, Vector2 v2, Vector2 v3)
        {
        }
    }
}
