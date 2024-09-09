using System.Collections.Generic;
using System.IO;

using Tokamak.Core.Utilities;

using Tokamak.Tritium.Pipelines;

namespace Tokamak.OGL
{
    internal class PipelineFactory : IFactory<IPipeline>
    {
        private readonly PipelineConfig m_config;

        private OpenGLLayer m_apiLayer;
        
        public PipelineFactory(OpenGLLayer apiLayer, PipelineConfig config)
        {
            m_apiLayer = apiLayer;
            m_config = config;
        }

        public void Dispose()
        {
        }

        private Shader GetGlShader()
        {
            var compilers = new List<ShaderCompiler>();
            var glShader = new Shader(m_apiLayer);

            try
            {
                foreach (var shader in m_config.Shaders)
                {
                    // TODO: Replace with VFS
                    string source = File.ReadAllText(shader.Path);
                    var comp = new ShaderCompiler(m_apiLayer, shader.Type, source);

                    //byte[] data = File.ReadAllBytes(shader.Path);
                    //var comp = new ShaderCompiler(m_apiLayer, shader.Type, data);

                    compilers.Add(comp);

                    m_apiLayer.GL.AttachShader(glShader.Handle, comp.Handle);
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
                    m_apiLayer.GL.DetachShader(glShader.Handle, comp.Handle);
                    comp.Dispose();
                }

                // Only dispose if there's a problem.
                glShader?.Dispose();
            }
        }

        public IPipeline Build()
        {
            Shader glShader = GetGlShader();

            var rval = new Pipeline(m_apiLayer, glShader)
            {
                DepthTest = m_config.DepthTest,
                Culling = m_config.Culling,
                Primitive = m_config.Primitive.ToGLPrimitive()
            };

            return rval;
        }
    }
}
