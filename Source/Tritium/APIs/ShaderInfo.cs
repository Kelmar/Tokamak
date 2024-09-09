namespace Tokamak.Tritium.APIs
{
    public record class ShaderInfo
    {
        public required ShaderType Type { get; init; }

        public required string Path { get; init; }
    }
}
