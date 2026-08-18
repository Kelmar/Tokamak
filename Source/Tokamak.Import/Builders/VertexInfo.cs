using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Import.Builders
{
    public record VertexInfo
    {
        public Vector3 Vector { get; set; }

        public Vector3 Normal { get; set; }

        public Vector2 TexCoord { get; set; }

        public Vector4 Color { get; set; }

        public List<BoneWeighting> BoneWeights
        {
            get;
            set => field = value ?? [];
        } = [];
    }
}
