using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Import.Builders;

public record BoneInfo
{
    public required string Name { get; init; }
    
    public required Matrix4x4 Transform { get; init; }

    public List<BoneInfo> Children
    {
        get;
        init => field = value ?? [];
    } = [];
}