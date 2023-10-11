using System;
using System.Collections.Generic;
using System.Linq;

using OpenTK.Graphics.OpenGL4;

using Tokamak.Mathematics;
using Tokamak.Buffer;

using TokPrimType = Tokamak.PrimitiveType;
using TokPixelFormat = Tokamak.Formats.PixelFormat;

namespace Tokamak.OGL
{
    public class GLDevice : Device
    {
        private readonly TextureObject m_whiteTexture;

        public GLDevice()
        {
            GL.ClearColor(0, 0, 0, 1);
            GL.Disable(EnableCap.DepthTest);

            // Create a default 1x1 white texture as not all drivers will do this.
            m_whiteTexture = new TextureObject(TokPixelFormat.FormatR8G8B8A8, new Point(1, 1));

            var bits = new Bitmap(new Point(1, 1), TokPixelFormat.FormatR8G8B8A8);

            Array.Fill<byte>(bits.Data, 255);

            m_whiteTexture.Set(bits);

            Monitors = EnumerateMonitors().ToList();
        }

        public override void Dispose()
        {
            m_whiteTexture.Dispose();

            base.Dispose();
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

        private IEnumerable<Monitor> EnumerateMonitors()
        {
            var mons = OpenTK.Windowing.Desktop.Monitors.GetMonitors();

            foreach (var m in mons)
            {
                var loc = new Point(m.WorkArea.Min.X, m.WorkArea.Min.Y);
                var size = new Point(m.WorkArea.Size.X, m.WorkArea.Size.Y);

                yield return new Monitor
                {
                    DPI = new Point((int)Math.Round(m.HorizontalDpi), (int)Math.Round(m.VerticalDpi)),
                    RawDPI = new System.Numerics.Vector2(m.HorizontalRawDpi, m.VerticalRawDpi),
                    WorkArea = new Rect(loc, size)
                };
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

        public override void ClearBoundTexture()
        {
            // Reset to our 1x1 white texture to keep samplers in shaders happy.
            m_whiteTexture.Activate();
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