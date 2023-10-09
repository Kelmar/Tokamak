using OpenTK.Graphics.OpenGL4;

using Tokamak.Mathematics;
using Tokamak.Buffer;

using TokPrimType = Tokamak.PrimitiveType;
using TokPixelFormat = Tokamak.Formats.PixelFormat;

namespace Tokamak.OGL
{
    public class GLDevice : Device
    {
        public GLDevice()
        {
            GL.ClearColor(0, 0, 0, 1);
            GL.Disable(EnableCap.DepthTest);
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

        public override ITextureObject GetTextureObject(TokPixelFormat format, Point size)
        {
            return new TextureObject(format, size);
        }

        public override IShaderFactory GetShaderFactory()
        {
            return new ShaderFactory();
        }

        public override void DrawArrays(TokPrimType primative, int vertexOffset, int vertexCount)
        {
            GL.DrawArrays(primative.ToGLPrimitive(), vertexOffset, vertexCount);
        }
    }
}