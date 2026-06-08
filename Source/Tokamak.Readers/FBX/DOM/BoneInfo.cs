using System;
using System.Collections.Generic;
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

        public required int[] Indicies { get; init; }

        public required float[] Weights { get; init; }
    }
}
