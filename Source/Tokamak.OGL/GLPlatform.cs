using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Silk.NET.OpenGL;
using Silk.NET.Core.Contexts;
using Silk.NET.Windowing;

using Stashbox;

using Tokamak.Core;
using Tokamak.Mathematics;
using Tokamak.Buffer;

using TokPixelFormat = Tokamak.Formats.PixelFormat;
using Tokamak.Core.Utilities;

namespace Tokamak.OGL
{
    public class GLPlatform : Platform
    {
        public GLPlatform(IGLContext context)
            : base()
        {
            GL = GL.GetApi(context);

            // Need to figure out how to abstract these.
            GL.Enable(EnableCap.Blend);

            // A good blending function for 2D font antialiasing.
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
        }

        public override void Dispose()
        {
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

        protected override IPipelineFactory GetPipelineFactory(PipelineConfig config)
        {
            return new PipelineFactory(this, config);
        }

        public override IVertexBuffer<T> GetVertexBuffer<T>(BufferType type)
        {
            return new VertexBuffer<T>(this, type);
        }

        public override ITextureObject GetTextureObject(TokPixelFormat format, Point size)
        {
            //return new TextureObject(this, format, size);
            return null;
        }

        public override IElementBuffer GetElementBuffer(BufferType type)
        {
            return new ElementBuffer(this, type);
        }
    }
}