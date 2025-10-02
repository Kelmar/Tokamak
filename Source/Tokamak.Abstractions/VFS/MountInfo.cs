namespace Tokamak.VFS.Abstractions
{
    public record MountInfo
    {
        public required string Root { get; init; }

        public required string Type { get; init; }
    }
}
