using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        [NotMapped]
        public long? ParentBoneId { get; init; }

        [NotMapped]
        public required int[] Indices { get; init; }

        [NotMapped]
        public required float[] Weights { get; init; }

        [Column("Lcl Translate")]
        public Vector3 Location { get; set; } = Vector3.Zero;

        [Column("Lcl Rotation")]
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        [Column("Lcl Scaling")]
        public Vector3 Scaling { get; set; } = Vector3.One;
    }
}
