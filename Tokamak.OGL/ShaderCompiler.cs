using System;

using OpenTK.Graphics.OpenGL4;

using GLShaderType = OpenTK.Graphics.OpenGL4.ShaderType;

namespace Tokamak.OGL
{
    /// <summary>
    /// Compiler for GLSL shaders.
    /// </summary>
    internal class ShaderCompiler : IDisposable
    {
        public ShaderCompiler(ShaderType type, string source)
        {
            Type = type switch
            {
                ShaderType.Fragment => GLShaderType.FragmentShader,
                ShaderType.Vertex => GLShaderType.VertexShader,
                ShaderType.Geometry => GLShaderType.GeometryShader,
                ShaderType.Compute => GLShaderType.ComputeShader,
                _ => throw new Exception($"Unknown shader type: {type}")
            };

            Handle = GL.CreateShader(Type);
            
            GL.ShaderSource(Handle, source);

            Compile();
        }

        public void Dispose()
        {
            if (Handle != 0)
                GL.DeleteShader(Handle);
        }

        public GLShaderType Type { get; }

        public int Handle { get; }

        private void Compile()
        {
            GL.CompileShader(Handle);

            GL.GetShader(Handle, ShaderParameter.CompileStatus, out int status);

            if (status != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(Handle);
                throw new Exception($"Error compiling shader {Type}: {infoLog}");
            }
        }
    }
}
