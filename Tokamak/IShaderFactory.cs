using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak
{
    /// <summary>
    /// Factory for loading new shaders.
    /// </summary>
    /// <remarks>
    /// At the moment the shader source is dependent on the underlying 
    /// graphics system. (I.e. OpenGL vs. Direct X)
    /// 
    /// TODO: Implement a generic shader system that can be compiled to
    /// an underlying graphics subsystem type?  (Or do they all support GLSL now?)
    /// 
    /// HLSL?
    /// 
    /// Metal looks to be able to transpile stuff that is 
    /// compiled with an LLVM IR shader?
    /// 
    /// SPIR-V
    /// </remarks>
    public interface IShaderFactory : IDisposable
    {
        void AddShaderSource(ShaderType type, string source);

        void AddShaderSource(ShaderType type, Stream source)
        {
            using var sr = new StreamReader(source);
            AddShaderSource(type, sr.ReadToEnd());
        }

        IShader Build();
    }
}
