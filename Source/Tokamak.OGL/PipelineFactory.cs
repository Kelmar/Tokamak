using System.Collections.Generic;
using System.IO;

namespace Tokamak.OGL
{
    internal class PipelineFactory : IPipelineFactory
    {
        private readonly PipelineConfig m_config;

        private GLPlatform m_platform;
        
        public PipelineFactory(GLPlatform platform, PipelineConfig config)
        {
            m_platform = platform;
            m_config = config;
        }

        public void Dispose()
        {
        }

        private Shader GetGlShader()
        {
            var compilers = new List<ShaderCompiler>();

            var glShader = new Shader(m_platform);

            try
            {
                foreach (var shader in m_config.Shaders)
                {
                    // TODO: Replace with VFS
                    string source = File.ReadAllText(shader.Path);
                    var comp = new ShaderCompiler(m_platform, shader.Type, source);

                    //byte[] data = File.ReadAllBytes(shader.Path);
                    //var comp = new ShaderCompiler(m_platform, shader.Type, data);

                    compilers.Add(comp);

                    m_platform.GL.AttachShader(glShader.Handle, comp.Handle);
                }

                glShader.Link();

                // Passing ownership on to caller.
                Shader rval = glShader;
                glShader = null; // Prevent finally Dispose() call.
                return rval;
            }
            finally
            {
                // Always dispose of the compilers.
                foreach (var comp in compilers)
                {
                    m_platform.GL.DetachShader(glShader.Handle, comp.Handle);
                    comp.Dispose();
                }

                // Only dispose if there's a problem.
                glShader?.Dispose();
            }
        }

        public IPipeline Build()
        {
            Shader glShader = GetGlShader();

            var rval = new Pipeline(m_platform, glShader)
            {
                Culling = m_config.Culling,
                Primitive = m_config.Primitive.ToGLPrimitive()
            };

            return rval;
        }
    }
}
