namespace Tokamak
{
    public record ShaderInfo
    {
        public required ShaderType Type { get; init; }

        public required string Path { get; init; }
    }
}
