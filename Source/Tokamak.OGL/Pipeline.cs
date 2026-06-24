using System;
using System.Numerics;

using Silk.NET.OpenGL;

using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Pipelines;
using Tokamak.Tritium.Pipelines.Shaders;

using GLPrimType = Silk.NET.OpenGL.PrimitiveType;
using GLBlendFact = Silk.NET.OpenGL.BlendingFactor;

namespace Tokamak.OGL
{
    internal sealed class Pipeline : IPipeline
    {
        private readonly GL m_gl;

        private readonly Shader m_shader;
        private readonly Vector4 m_clearColor;

        public Pipeline(GL gl, Shader shader)
        {
            m_gl = gl;
            m_shader = shader;

            Uniforms = new UniformBinder(m_shader);
        }

        public void Dispose()
        {
            m_shader.Dispose();
            GC.SuppressFinalize(this);
        }

        public Color ClearColor
        {
            get => Color.FromVector(m_clearColor);
            init => m_clearColor = value.ToVector();
        }

        public dynamic Uniforms { get; }

        public bool EnableBlend { get; init; }

        public GLBlendFact SourceColorFactor { get; init; }

        public GLBlendFact DestinationColorFactor { get; init; }

        public GLBlendFact SourceAlphaFactor { get; init; }

        public GLBlendFact DestinationAlphaFactor { get; init; }

        public bool DepthTest { get; init; }

        public CullMode Culling { get; init; }

        public GLPrimType Primitive { get; set; }

        private void SetClearColor()
        {
            m_gl.ClearColor(m_clearColor.X, m_clearColor.Y, m_clearColor.Z, m_clearColor.W);
        }

        private void SetBlendMode()
        {
            if (EnableBlend)
            {
                m_gl.Enable(EnableCap.Blend);
                m_gl.BlendFuncSeparate(
                    SourceColorFactor, DestinationColorFactor,
                    SourceAlphaFactor, DestinationAlphaFactor);
            }
            else
            {
                m_gl.Disable(EnableCap.Blend);
            }
        }

        private void SetDepthTest()
        {
            if (DepthTest)
                m_gl.Enable(EnableCap.DepthTest);
            else
                m_gl.Disable(EnableCap.DepthTest);
        }

        private void SetCullingMode()
        {
            if (Culling == CullMode.None)
                m_gl.Disable(EnableCap.CullFace);
            else
            {
                m_gl.Enable(EnableCap.CullFace);

                switch (Culling)
                {
                case CullMode.Back:
                    m_gl.CullFace(TriangleFace.Back);
                    break;

                case CullMode.Front:
                    m_gl.CullFace(TriangleFace.Front);
                    break;

                case CullMode.FrontAndBack:
                    m_gl.CullFace(TriangleFace.FrontAndBack);
                    break;
                }
            }
        }
        public void Activate(ICommandList buffer)
        {
            var cmdBuffer = (CommandList)buffer;
            cmdBuffer.MakeActive(this);

            m_shader.Activate();

            SetClearColor();
            SetBlendMode();
            SetDepthTest();
            SetCullingMode();
        }
    }
}
