﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Silk.NET.OpenGL;
using Silk.NET.Core.Contexts;
using Silk.NET.Windowing;

using Tokamak.Mathematics;
using Tokamak.Buffer;

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

        protected override IPipelineFactory GetPipelineFactory(PipelineConfig config)
        {
            return new PipelineFactory(this, config);
        }

        public override ICommandList GetCommandList()
        {
            return new GLCommandList(GL, m_whiteTexture);
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
    }
}