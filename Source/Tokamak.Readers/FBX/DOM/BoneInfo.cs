using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Tokamak.Readers.FBX.DOM
{
    internal class BoneInfo : ResultRecord
    {
        /// <summary>
        /// The parent bone for this bone.
        /// </summary>
        /// <remarks>
        /// Null indicates that this is the root bone.
        /// </remarks>
        public long? ParentBoneId { get; init; }

        public required int[] Indices { get; init; }

        public required float[] Weights { get; init; }

        public Vector3 Location { get; set; } = Vector3.Zero;

        public Vector3 Rotation { get; set; } = Vector3.Zero;

        public Vector3 Scaling { get; set; } = Vector3.One;
    }
}
