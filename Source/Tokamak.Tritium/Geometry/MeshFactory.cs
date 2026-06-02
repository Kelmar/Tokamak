using System;

using Tokamak.Assets;
using Tokamak.Tritium.APIs;

namespace Tokamak.Tritium.Geometry
{
    internal class MeshFactory : IAssetFactory
    {
        private readonly IGraphicsLayer m_gfxLayer;

        public MeshFactory(IGraphicsLayer gfxLayer)
        {
            m_gfxLayer = gfxLayer;
        }

        public Type ForType => typeof(Mesh);

        public Asset Build()
        {
            return new Mesh(m_gfxLayer);
        }
    }
}
