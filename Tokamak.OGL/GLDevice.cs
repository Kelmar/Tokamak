using System;
using System.Numerics;

using OpenTK.Graphics.OpenGL;

using TokPrimType = Tokamak.PrimitiveType;
using GLPrimType = OpenTK.Graphics.OpenGL.PrimitiveType;
using Tokamak.Mathematics;

namespace Tokamak.OGL
{
    public class GLDevice : Device
    {
        public GLDevice()
        {
            GL.ClearColor(0, 0, 0, 1);
        }

        public override Rect Viewport 
        { 
            get => base.Viewport; 
            set
            {
                GL.Viewport(value.Left, value.Top, value.Extent.X, value.Extent.Y);
                base.Viewport = value;
            }
        }

        public override IVertexBuffer<T> GetVertexBuffer<T>()
            where T : struct
        {
            return new VertexBuffer<T>();
        }

        public override void Activate(IVertexBuffer buffer)
        {
            
        }

        public override void Activate(IShader shader)
        {
            
        }

        private GLPrimType ToGLPrim(TokPrimType primType)
        {
            return primType switch
            {
                TokPrimType.PointList => GLPrimType.Points,
                TokPrimType.LineList => GLPrimType.Lines,
                TokPrimType.LineStrip => GLPrimType.LineStrip,
                TokPrimType.TrangleList => GLPrimType.Triangles,
                TokPrimType.TrangleStrip => GLPrimType.TriangleStrip,
                _ => GLPrimType.Points
            };
        }

        public override void DrawArrays(TokPrimType primative, int vertexOffset, int vertexCount)
        {
            GLPrimType prim = ToGLPrim(primative);

            GL.DrawArrays(prim, vertexOffset, vertexCount);
        }
    }
}