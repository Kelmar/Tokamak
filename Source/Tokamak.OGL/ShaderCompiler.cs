using System;

using Silk.NET.OpenGL;

using ShaderType = Tokamak.Tritium.Pipelines.Shaders.ShaderType;
using GLShaderType = Silk.NET.OpenGL.ShaderType;

namespace Tokamak.OGL
{
    /// <summary>
    /// Compiler for GLSL shaders.
    /// </summary>
    internal unsafe class ShaderCompiler : IDisposable
    {
        private readonly OpenGLLayer m_apiLayer;

        private readonly uint m_handle;

        /// <summary>
        /// Load/Compile a shader from source code.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="type"></param>
        /// <param name="source"></param>
        public ShaderCompiler(OpenGLLayer apiLayer, ShaderType type, string source)
        {
            m_apiLayer = apiLayer;

            Type = type.ToGLShaderType();

            m_handle = m_apiLayer.GL.CreateShader(Type);

            m_apiLayer.GL.ShaderSource(m_handle, source);

            Compile();
        }

        /// <summary>
        /// Load/Compile a precompiled binary shader.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public ShaderCompiler(OpenGLLayer apiLayer, ShaderType type, in ReadOnlySpan<byte> data)
        {
            m_apiLayer = apiLayer;

            Type = type.ToGLShaderType();

            m_handle = m_apiLayer.GL.CreateShader(Type);

            fixed (uint* h = &m_handle)
            {
                uint len = (uint)data.Length;
                m_apiLayer.GL.ShaderBinary(1, h, GLEnum.SpirVBinary, data, len);
            }

            uint constIndex = 0;
            uint constValue = 0;

            m_apiLayer.GL.SpecializeShader(m_handle, "main", 0, ref constIndex, ref constValue);

            Compile();
        }

        public void Dispose()
        {
            if (m_handle != 0)
                m_apiLayer.GL.DeleteShader(m_handle);
        }

        public GLShaderType Type { get; }

        public uint Handle => m_handle;

        private void Compile()
        {
            m_apiLayer.GL.CompileShader(m_handle);

            m_apiLayer.GL.GetShader(m_handle, GLEnum.CompileStatus, out int status);

            if (status == 0)
            {
                string infoLog = m_apiLayer.GL.GetShaderInfoLog(m_handle);
                throw new Exception($"Error compiling shader {Type}: {infoLog}");
            }
        }
    }
}
