using System;

using Silk.NET.OpenGL;

using GLShaderType = Silk.NET.OpenGL.ShaderType;

namespace Tokamak.OGL
{
    /// <summary>
    /// Compiler for GLSL shaders.
    /// </summary>
    internal unsafe class ShaderCompiler : IDisposable
    {
        private readonly GLPlatform m_device;

        private readonly uint m_handle;

        /// <summary>
        /// Load/Compile a shader from source code.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="type"></param>
        /// <param name="source"></param>
        public ShaderCompiler(GLPlatform device, ShaderType type, string source)
        {
            m_device = device;

            Type = type.ToGLShaderType();

            m_handle = m_device.GL.CreateShader(Type);

            m_device.GL.ShaderSource(m_handle, source);

            Compile();
        }

        /// <summary>
        /// Load/Compile a precompiled binary shader.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public ShaderCompiler(GLPlatform device, ShaderType type, ReadOnlySpan<byte> data)
        {
            m_device = device;

            Type = type.ToGLShaderType();

            m_handle = m_device.GL.CreateShader(Type);

            fixed (uint* h = &m_handle)
            {
                uint len = (uint)data.Length;
                m_device.GL.ShaderBinary(1, h, GLEnum.SpirVBinary, data, len);
            }

            uint constIndex = 0;
            uint constValue = 0;

            m_device.GL.SpecializeShader(m_handle, "main", 0, ref constIndex, ref constValue);

            Compile();
        }

        public void Dispose()
        {
            if (m_handle != 0)
                m_device.GL.DeleteShader(m_handle);
        }

        public GLShaderType Type { get; }

        public uint Handle => m_handle;

        private void Compile()
        {
            m_device.GL.CompileShader(m_handle);

            m_device.GL.GetShader(m_handle, GLEnum.CompileStatus, out int status);

            if (status == 0)
            {
                string infoLog = m_device.GL.GetShaderInfoLog(m_handle);
                throw new Exception($"Error compiling shader {Type}: {infoLog}");
            }
        }
    }
}
