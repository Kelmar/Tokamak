using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Tokamak.Import.FBX.DOM
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

        /// <summary>
        /// An assigned index that we will sort the bones on.
        /// </summary>
        /// <remarks>
        /// Note that this will be a consecutive index of the
        /// bones relative to each other.  Unlike the somewhat
        /// random ID that we read from the FBX file.
        /// 
        /// This is also the index the bone will appear in the
        /// list that we send to the builder classes.
        /// </remarks>
        [NotMapped]
        public int Index { get; set; } = -1;

        /// <summary>
        /// The index of the parent bone.
        /// </summary>
        /// <remarks>
        /// This is NULL if this is a root bone.
        /// </remarks>
        public int? ParentIndex { get; set; } = null;

        /// <summary>
        /// List of vertex indices that this bone affects.
        /// </summary>
        [NotMapped]
        public required int[] Indices { get; init; }

        /// <summary>
        /// The list of weightings for each vertex.
        /// </summary>
        [NotMapped]
        public required float[] Weights { get; init; }

        public Matrix4x4 Transform { get; set; } = Matrix4x4.Identity;

        [Column("Lcl Translate")]
        public Vector3 Location { get; set; } = Vector3.Zero;

        [Column("Lcl Rotation")]
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        [Column("Lcl Scaling")]
        public Vector3 Scaling { get; set; } = Vector3.One;
    }
}
