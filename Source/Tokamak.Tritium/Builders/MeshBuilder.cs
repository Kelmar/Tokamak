using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Assets;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Builders
{
    internal class MeshBuilder : IMeshBuilder
    {
        private readonly AssetManager m_assetManager;
        private readonly IGraphicsLayer m_gfxLayer;

        public MeshBuilder(AssetManager assetManager, IGraphicsLayer gfxLayer)
        {
            m_assetManager = assetManager;
            m_gfxLayer = gfxLayer;
        }

        public string Name { get; private set; } = String.Empty;

        public List<Polygon> Polygons { get; private set; } = [];

        public IMeshBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public IMeshBuilder WithPolygons(IEnumerable<Polygon> polys)
        {
            Polygons = polys.SelectMany(p => p.SplitIntoTriangles()).ToList();
            return this;
        }

        private void Validate()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Name, nameof(Name));

            if (Polygons.Count == 0)
                throw new ArgumentException(nameof(Polygons));
        }

        public void Build()
        {
            Validate();

            var mesh = new Mesh(m_gfxLayer);

            try
            {
                mesh.SetData(Polygons);

                m_assetManager.RegisterAsset(Name, mesh);
            }
            catch
            {
                mesh.Dispose();
                throw;
            }
        }
    }
}
