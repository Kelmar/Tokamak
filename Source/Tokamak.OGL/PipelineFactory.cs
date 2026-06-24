using System.Collections.Generic;

using Tokamak.Utilities;

using Tokamak.Tritium.Pipelines;

using Silk.NET.OpenGL;

namespace Tokamak.OGL
{
    internal class PipelineFactory : IFactory<IPipeline>
    {
        private readonly PipelineConfig m_config;

        private readonly GL m_gl;
        
        public PipelineFactory(GL gl, PipelineConfig config)
        {
            m_gl = gl;
            m_config = config;
        }

        public void Dispose()
        {
        }

        private Shader GetGlShader()
        {
            var compilers = new List<ShaderCompiler>();
            var glShader = new Shader(m_gl);
            bool disposeShader = true;

            try
            {
                foreach (var shaderSource in m_config.ShaderSources)
                {
                    ShaderCompiler comp;

                    if (shaderSource.Precompiled)
                        comp = new ShaderCompiler(m_gl, shaderSource.Type, shaderSource.GetData());
                    else
                        comp = new ShaderCompiler(m_gl, shaderSource.Type, shaderSource.GetSourceCode());

                    compilers.Add(comp);

                    m_gl.AttachShader(glShader.Handle, comp.Handle);
                }

                glShader.Link();

                // Passing ownership on to caller.
                Shader rval = glShader;
                disposeShader = false; // Prevent finally Dispose() call.
                return rval;
            }
            finally
            {
                // Always dispose of the compilers.
                foreach (var comp in compilers)
                {
                    m_gl.DetachShader(glShader.Handle, comp.Handle);
                    comp.Dispose();
                }

                // Only dispose if there's a problem.
                if (disposeShader)
                    glShader.Dispose();
            }
        }

        public IPipeline Build()
        {
            Shader glShader = GetGlShader();

            return new Pipeline(m_gl, glShader)
            {
                DepthTest = m_config.DepthTest,
                Culling = m_config.Culling,
                Primitive = m_config.Primitive.ToGLPrimitive(),
                EnableBlend = m_config.Blending,
                SourceColorFactor = m_config.SourceColorBlendFactor.ToGLBlendFact(),
                DestinationColorFactor = m_config.DestinationColorBlendFactor.ToGLBlendFact(),
                SourceAlphaFactor = m_config.SourceBlendFactorAlpha.ToGLBlendFact(),
                DestinationAlphaFactor = m_config.DestinationAlphaBlendFactor.ToGLBlendFact(),
            };
        }
    }
}
