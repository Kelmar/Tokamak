using System;
using System.Text;

namespace Tokamak.Tritium.Pipelines.Shaders
{
    public class ShaderSourceCode : IShaderSource
    {
        private readonly string m_code;

        public ShaderSourceCode(ShaderType type, string code)
        {
            Type = type;
            m_code = code;
        }

        public ShaderType Type { get; }

        public bool Precompiled => false;

        public string GetSourceCode() => m_code;

        public Span<byte> GetData() => Encoding.ASCII.GetBytes(m_code);
    }
}
