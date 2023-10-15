using System;

//using OpenTK.Graphics.OpenGL4;
using Silk.NET.OpenGL;

//using GLShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

using GLShaderType = Silk.NET.OpenGL.ShaderType;

namespace Tokamak.OGL
{
    /// <summary>
    /// Compiler for GLSL shaders.
    /// </summary>
    internal class ShaderCompiler : IDisposable
    {
        private readonly GLPlatform m_device;

        public ShaderCompiler(GLPlatform device, ShaderType type, string source)
        {
            m_device = device;

            Type = type switch
            {
                ShaderType.Fragment => GLShaderType.FragmentShader,
                ShaderType.Vertex => GLShaderType.VertexShader,
                ShaderType.Geometry => GLShaderType.GeometryShader,
                ShaderType.Compute => GLShaderType.ComputeShader,
                _ => throw new Exception($"Unknown shader type: {type}")
            };

            Handle = m_device.GL.CreateShader(Type);

            m_device.GL.ShaderSource(Handle, source);

            Compile();
        }

        public void Dispose()
        {
            if (Handle != 0)
                m_device.GL.DeleteShader(Handle);
        }

        public GLShaderType Type { get; }

        public uint Handle { get; }

        private void Compile()
        {
            m_device.GL.CompileShader(Handle);

            m_device.GL.GetShader(Handle, GLEnum.CompileStatus, out int status);

            if (status == 0)
            {
                string infoLog = m_device.GL.GetShaderInfoLog(Handle);
                throw new Exception($"Error compiling shader {Type}: {infoLog}");
            }
        }
    }
}
