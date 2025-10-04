using System;
using System.IO;
using System.Text;

namespace Tokamak.Tritium.Pipelines.Shaders
{
    public class ShaderSourceFile : IShaderSource
    {
        private readonly string m_path;

        public ShaderSourceFile(ShaderType type, string path)
        {
            Type = type;
            m_path = path;

            // TODO: Fix this, not using the VFS

            if (!File.Exists(m_path))
                throw new FileNotFoundException("Shader file not found.", m_path);
        }

        public ShaderType Type { get; }

        public bool Precompiled => false;

        public string GetSourceCode() => File.ReadAllText(m_path);

        public Span<byte> GetData() => Encoding.ASCII.GetBytes(GetSourceCode());
    }
}
