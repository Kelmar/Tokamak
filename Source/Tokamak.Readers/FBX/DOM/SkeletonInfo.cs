using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Tokamak.Readers.FBX.DOM
{
    internal class SkeletonInfo : ResultRecord
    {
        public long? MeshId { get; init; }

        public required List<BoneInfo> Bones { get; init; }

        public Vector3 Location { get; set; } = Vector3.Zero;

        public Vector3 Rotation { get; set; } = Vector3.Zero;

        public Vector3 Scaling { get; set; } = Vector3.One;
    }
}
