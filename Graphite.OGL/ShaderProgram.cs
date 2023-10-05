using System;
using System.Collections.Generic;

using Numerics = System.Numerics;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Graphite.OGL
{
    internal class ShaderProgram : IDisposable
    {
        private readonly int m_handle;

        private readonly IDictionary<string, int> m_uniforms = new Dictionary<string, int>();

        public ShaderProgram()
        {
            m_handle = GL.CreateProgram();

            using var vertex = new Shader(ShaderType.VertexShader, Shaders.Vertex);
            using var fragment = new Shader(ShaderType.FragmentShader, Shaders.Fragment);

            vertex.AttachTo(m_handle);
            fragment.AttachTo(m_handle);

            GL.LinkProgram(m_handle);

            GL.GetProgram(m_handle, GetProgramParameterName.LinkStatus, out var code);

            if (code != (int)All.True)
            {
                string info = GL.GetProgramInfoLog(m_handle);
                GL.DeleteProgram(m_handle);
                throw new Exception($"Error linking shader program: {info}");
            }

            GL.GetProgram(m_handle, GetProgramParameterName.ActiveUniforms, out int uniCount);

            for (int i = 0; i < uniCount; ++i)
            {
                string key = GL.GetActiveUniform(m_handle, i, out _, out _);

                m_uniforms[key] = GL.GetUniformLocation(m_handle, key);
            }
        }

        public void Dispose()
        {
            GL.DeleteProgram(m_handle);
        }

        public void Use() => GL.UseProgram(m_handle);

        public void SetInt(string name, int value)
        {
            Use();
            GL.Uniform1(m_uniforms[name], value);
        }

        public void SetFloat(string name, float value)
        {
            Use();
            GL.Uniform1(m_uniforms[name], value);
        }

        public void SetVector2(string name, ref Vector2 vector)
        {
            Use();
            GL.Uniform2(m_uniforms[name], ref vector);
        }

        public void SetVector3(string name, ref Vector3 vector)
        {
            Use();
            GL.Uniform3(m_uniforms[name], ref vector);
        }

        public void SetVector4(string name, ref Vector4 vector)
        {
            Use();
            GL.Uniform4(m_uniforms[name], ref vector);
        }

        public void SetMatrix4(string name, ref Matrix4 matrix)
        {
            Use();
            GL.UniformMatrix4(m_uniforms[name], true, ref matrix);
        }
    }
}
