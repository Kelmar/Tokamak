using System;

namespace Tokamak.Tritium.Pipelines.Shaders
{
    public interface IShaderSource
    {
        ShaderType Type { get; }

        bool Precompiled { get; }

        string GetSourceCode();

        Span<byte> GetData();
    }
}
