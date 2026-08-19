using System.Collections.Generic;

namespace Tokamak.Import.Builders;

public record SkeletonInfo
{ 
    public required string Name { get; init; }

    public List<BoneInfo> Bones
    {
        get;
        init => field = value ?? [];
    } = [];
};