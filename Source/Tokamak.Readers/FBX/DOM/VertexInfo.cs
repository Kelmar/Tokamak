using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Readers.FBX.DOM
{
    internal class VertexInfo
    {
        public class BoneWeighting
        {
            public long BoneIndex { get; set; }

            public float BoneWeight { get; set; }
        }

        public int Index { get; set; }

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
    }
}
