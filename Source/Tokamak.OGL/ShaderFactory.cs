using System.Collections.Generic;

namespace Tokamak.OGL
{
    internal class ShaderFactory : IShaderFactory
    {
        private readonly List<ShaderCompiler> m_compilers = new List<ShaderCompiler>();

        private Shader m_shader;
        private GLPlatform m_platform;

        public ShaderFactory(GLPlatform platform)
        {
            m_platform = platform;
            m_shader = new Shader(platform);
        }

        public void Dispose()
        {
            /*
             * This isn't perfect, but if we're disposing of the actual
             * OpenGL.Shader objects here, then we're probably going to
             * crash anyhow.
             */
            foreach (var comp in m_compilers)
                comp.Dispose();

            if (m_shader != null)
                m_shader.Dispose();
        }

        public void AddShaderSource(ShaderType type, string source)
        {
            var comp = new ShaderCompiler(m_platform, type, source);

            m_compilers.Add(comp);
        }

        public IShader Build()
        {
            foreach (var comp in m_compilers)
                m_platform.GL.AttachShader(m_shader.Handle, comp.Handle);

            m_shader.Link();

            foreach (var comp in m_compilers)
            {
                m_platform.GL.DetachShader(m_shader.Handle, comp.Handle);
                comp.Dispose();
            }

            m_compilers.Clear();

            var rval = m_shader;
            m_shader = null; // Passing off ownership to the caller.
            return rval;
        }
    }
}
