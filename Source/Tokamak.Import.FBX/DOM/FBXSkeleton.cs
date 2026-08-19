using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Import.FBX.DOM;

internal class FBXSkeleton : ResultRecord
{
    public long? MeshId { get; init; }

    public required List<FBXBone> Bones { get; init; }

    public Vector3 Location { get; set; } = Vector3.Zero;

    public Vector3 Rotation { get; set; } = Vector3.Zero;

    public Vector3 Scaling { get; set; } = Vector3.One;
}