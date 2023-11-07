using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Silk.NET.OpenGL;
using Silk.NET.Core.Contexts;
using Silk.NET.Windowing;

using Tokamak.Mathematics;
using Tokamak.Buffer;

using TokPrimType = Tokamak.PrimitiveType;
using TokPixelFormat = Tokamak.Formats.PixelFormat;

namespace Tokamak.OGL
{
    public class GLPlatform : Platform
    {
        private readonly TextureObject m_whiteTexture;

        public GLPlatform(IGLContextSource context)
        {
            GL = GL.GetApi(context);

            var defaultState = new RenderState
            {
                CullFaces = false,
                UseDepthTest = false,
                ClearColor = Color.Black
            };

            SetRenderState(defaultState);

            // Need to figure out how to abstract these.
            GL.Enable(EnableCap.Blend);

            // A good blending function for 2D font antialiasing.
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            
            // Create a default 1x1 white texture as not all drivers will do this.
            m_whiteTexture = new TextureObject(this, TokPixelFormat.FormatR8G8B8A8, new Point(1, 1));

            Array.Fill<byte>(m_whiteTexture.Bitmap.Data, 255);
            m_whiteTexture.Refresh();

            Monitors = EnumerateMonitors().ToList();
        }

        public override void Dispose()
        {
            m_whiteTexture.Dispose();
            GL.Dispose();

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

        protected override IPipelineFactory GetPipelineFactory(PipelineConfig config)
        {
            return new PipelineFactory(this, config);
        }

        public override void SetRenderState(RenderState state)
        {
            if (state.UseDepthTest)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);

            if (state.CullFaces)
                GL.Enable(EnableCap.CullFace);
            else
                GL.Disable(EnableCap.CullFace);

            GL.ClearColor(state.ClearColor.Red, state.ClearColor.Green, state.ClearColor.Blue, state.ClearColor.Alpha);
        }

        public override void ClearBuffers(GlobalBuffer buffers)
        {
            ClearBufferMask flags = 0;

            flags |= buffers.HasFlag(GlobalBuffer.ColorBuffer) ? ClearBufferMask.ColorBufferBit : 0;
            flags |= buffers.HasFlag(GlobalBuffer.DepthBuffer) ? ClearBufferMask.DepthBufferBit : 0;
            flags |= buffers.HasFlag(GlobalBuffer.StencilBuffer) ? ClearBufferMask.StencilBufferBit : 0;

            if ((int)flags != 0)
                GL.Clear(flags);
        }

        public override IVertexBuffer<T> GetVertexBuffer<T>(BufferType type)
        {
            return new VertexBuffer<T>(this, type);
        }

        public override ITextureObject GetTextureObject(TokPixelFormat format, Point size)
        {
            return new TextureObject(this, format, size);
        }

        public override IElementBuffer GetElementBuffer(BufferType type)
        {
            return new ElementBuffer(this, type);
        }

        public override void ClearBoundTexture()
        {
            // Reset to our 1x1 white texture to keep samplers in shaders happy.
            m_whiteTexture.Activate();
        }

        public override void DrawArrays(TokPrimType primative, int vertexOffset, int vertexCount)
        {
            GL.DrawArrays(primative.ToGLPrimitive(), vertexOffset, (uint)vertexCount);
        }

        public override void DrawElements(TokPrimType primitive, int length)
        {
            GL.DrawElements(primitive.ToGLPrimitive(), (uint)length, DrawElementsType.UnsignedInt, 0);
        }
    }
}