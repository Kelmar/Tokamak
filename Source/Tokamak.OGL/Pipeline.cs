using System;
using System.Numerics;

using Silk.NET.OpenGL;

using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Pipelines;

using GLPrimType = Silk.NET.OpenGL.PrimitiveType;
using GLBlendFact = Silk.NET.OpenGL.BlendingFactor;

namespace Tokamak.OGL
{
    internal sealed class Pipeline : IPipeline
    {
        private readonly OpenGLLayer m_apiLayer;

        private readonly Shader m_shader;
        private readonly Vector4 m_clearColor;

        public Pipeline(OpenGLLayer apiLayer, Shader shader)
        {
            m_apiLayer = apiLayer;
            m_shader = shader;
        }

        public void Dispose()
        {
            m_shader.Dispose();
            GC.SuppressFinalize(this);
        }

        public Color ClearColor
        {
            get => (Color)m_clearColor;
            init => m_clearColor = (Vector4)value;
        }

        public bool EnableBlend { get; init; }

        public GLBlendFact SourceFactor { get; init; }

        public GLBlendFact DestinationFactor { get; init; }

        public bool DepthTest { get; init; }

        public CullMode Culling { get; init; }

        public GLPrimType Primitive { get; set; }

        private void SetClearColor()
        {
            m_apiLayer.GL.ClearColor(m_clearColor.X, m_clearColor.Y, m_clearColor.Z, m_clearColor.W);
        }

        private void SetBlendMode()
        {
            if (EnableBlend)
            {
                m_apiLayer.GL.Enable(EnableCap.Blend);
                m_apiLayer.GL.BlendFunc(SourceFactor, DestinationFactor);
            }
            else
            {
                m_apiLayer.GL.Disable(EnableCap.Blend);
            }
        }

        private void SetDepthTest()
        {
            if (DepthTest)
                m_apiLayer.GL.Enable(EnableCap.DepthTest);
            else
                m_apiLayer.GL.Disable(EnableCap.DepthTest);
        }

        private void SetCullingMode()
        {
            if (Culling == CullMode.None)
                m_apiLayer.GL.Disable(EnableCap.CullFace);
            else
            {
                m_apiLayer.GL.Enable(EnableCap.CullFace);

                switch (Culling)
                {
                case CullMode.Back:
                    m_apiLayer.GL.CullFace(TriangleFace.Back);
                    break;

                case CullMode.Front:
                    m_apiLayer.GL.CullFace(TriangleFace.Front);
                    break;

                case CullMode.FrontAndBack:
                    m_apiLayer.GL.CullFace(TriangleFace.FrontAndBack);
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
