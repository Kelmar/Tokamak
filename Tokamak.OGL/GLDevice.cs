using OpenTK.Graphics.OpenGL;

using TokPrimType = Tokamak.PrimitiveType;

using Tokamak.Mathematics;
using Tokamak.Buffer;

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

        public override IVertexBuffer<T> GetVertexBuffer<T>(BufferType type)
            where T : struct
        {
            return new VertexBuffer<T>(type);
        }

        public override void Activate(IVertexBuffer buffer)
        {
            
        }

        public override void Activate(IShader shader)
        {
            
        }

        public override void DrawArrays(TokPrimType primative, int vertexOffset, int vertexCount)
        {
            GL.DrawArrays(primative.ToGLPrimitive(), vertexOffset, vertexCount);
        }
    }
}