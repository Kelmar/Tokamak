﻿using System;

using Silk.NET.OpenGL;

using GLPrimType = Silk.NET.OpenGL.PrimitiveType;

namespace Tokamak.OGL
{
    internal sealed class Pipeline : IPipeline
    {
        private readonly Shader m_shader;

        public Pipeline(GLPlatform platform, Shader shader)
        {
            Platform = platform;
            m_shader = shader;
        }

        public void Dispose()
        {
            m_shader.Dispose();
            GC.SuppressFinalize(this);
        }

        public GLPlatform Platform { get; }

        public CullMode Culling { get; init; }

        public GLPrimType Primitive { get; set; }

        private void SetCullingMode()
        {
            if (Culling == CullMode.None)
                Platform.GL.Disable(EnableCap.CullFace);
            else
            {
                Platform.GL.Enable(EnableCap.CullFace);

                switch (Culling)
                {
                case CullMode.Back:
                    Platform.GL.CullFace(TriangleFace.Back);
                    break;

                case CullMode.Front:
                    Platform.GL.CullFace(TriangleFace.Front);
                    break;

                case CullMode.FrontAndBack:
                    Platform.GL.CullFace(TriangleFace.FrontAndBack);
                    break;
                }
            }
        }

        public void Activate(ICommandList buffer)
        {
            var cmdBuffer = (GLCommandList)buffer;
            cmdBuffer.MakeActive(this);

            m_shader.Activate();

            SetCullingMode();
        }
    }
}