using System;
using System.Collections.Generic;
using System.Linq;

using Silk.NET.OpenGL;
using Silk.NET.Core.Contexts;

using Tokamak.Mathematics;
using Tokamak.Buffer;

using TokPrimType = Tokamak.PrimitiveType;
using TokPixelFormat = Tokamak.Formats.PixelFormat;
using System.Runtime.InteropServices;
using Silk.NET.Windowing;
using System.Numerics;
using Silk.NET.Maths;

namespace Tokamak.OGL
{
    public class GLDevice : Device
    {
        private readonly TextureObject m_whiteTexture;

        public GLDevice(IGLContextSource context)
        {
            GL = GL.GetApi(context);

            // Need to figure out how to abstract these.
            GL.ClearColor(0, 0, 0, 1);
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);

            // A good blending function for 2D font antialiasing.
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            
            // Create a default 1x1 white texture as not all drivers will do this.
            m_whiteTexture = new TextureObject(this, TokPixelFormat.FormatR8G8B8A8, new Point(1, 1));

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

        public GL GL { get; }

        public override Rect Viewport
        {
            get => base.Viewport;
            set
            {
                GL.Viewport(value.Left, value.Top, (uint)value.Size.X, (uint)value.Size.Y);
                base.Viewport = value;
            }
        }

        private IEnumerable<Monitor> EnumerateMonitors()
        {
            var platform = Window.GetWindowPlatform(false);

            if (platform == null)
                throw new Exception("Unable to get window platform.");

            foreach (var m in platform.GetMonitors())
            {
                // Silk doesn't return the DPI info yet, hard coded for now.

                yield return new Monitor
                {
                    Index = m.Index,
                    IsMain = m.Index == 0, // For now we assume it's the first monitor
                    Gamma = m.Gamma,
                    DPI = new Point(192, 192),
                    RawDPI = new Vector2(192, 192),
                    WorkArea = m.Bounds
                };
            }
        }

        public override IVertexBuffer<T> GetVertexBuffer<T>(BufferType type)
            //where T : unmanaged
        {
            return new VertexBuffer<T>(this, type);
        }

        public override ITextureObject GetTextureObject(TokPixelFormat format, Point size)
        {
            return new TextureObject(this, format, size);
        }

        public override void ClearBoundTexture()
        {
            // Reset to our 1x1 white texture to keep samplers in shaders happy.
            m_whiteTexture.Activate();
        }

        public override IShaderFactory GetShaderFactory()
        {
            return new ShaderFactory(this);
        }

        public override void DrawArrays(TokPrimType primative, int vertexOffset, int vertexCount)
        {
            GL.DrawArrays(primative.ToGLPrimitive(), vertexOffset, (uint)vertexCount);
        }
    }
}