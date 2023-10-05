using System;

using OpenTK.Graphics.OpenGL4;

namespace Graphite.OGL
{
    internal sealed class Shader : IDisposable
    {
        private readonly int m_handle;

        private int m_currentProg = 0;

        public Shader(ShaderType type, string source)
        {
            m_handle = GL.CreateShader(type);

            GL.ShaderSource(m_handle, source);

            GL.CompileShader(m_handle);

            GL.GetShader(m_handle, ShaderParameter.CompileStatus, out var code);

            if (code != (int)All.True)
            {
                string info = GL.GetShaderInfoLog(m_handle);
                GL.DeleteShader(m_handle);
                throw new Exception($"Error occured compiling shader: {info}");
            }
        }

        public void Dispose()
        {
            Detach();
            GL.DeleteShader(m_handle);
        }

        public void Detach()
        {
            if (m_currentProg != 0)
            {
                GL.DetachShader(m_currentProg, m_handle);
                m_currentProg = 0;
            }
        }

        public void AttachTo(int program)
        {
            Detach();

            GL.AttachShader(program, m_handle);
            m_currentProg = program;
        }
    }
}
