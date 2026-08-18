using System;
using System.Collections.Generic;
using System.Numerics;

using Tokamak.Import.Builders;

namespace Tokamak.Import.FBX.DOM
{
    internal class VertexInfo
    {
        public int Index { get; set; }

        public int FBXIndex { get; set; }

        public Vector3 Vertex { get; set; }

        public Vector3 Normal { get; set; }

        public Vector2 TexCoord { get; set; }

        /// <summary>
        /// Materials might not be loaded yet, so we just store the index
        /// until we're ready to convert to a proper reference.
        /// </summary>
        public int MaterialIndex { get; set; }

        public List<BoneWeighting> BoneWeights
        {
            get;
            set => field = value ?? [];
        } = [];

        public override int GetHashCode()
            => HashCode.Combine(Vertex, Normal, TexCoord, MaterialIndex, BoneWeights);
    }
}
