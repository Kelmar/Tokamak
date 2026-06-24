using System;
using System.Collections.Generic;

using Tokamak.Assets;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Builders
{
    internal partial class MeshBuilder : IMeshBuilder
    {

        private readonly PolygonBuilder m_polyBuilder;

        private readonly AssetManager m_assetManager;
        private readonly IGraphicsLayer m_gfxLayer;

        public MeshBuilder(AssetManager assetManager, IGraphicsLayer gfxLayer)
        {
            m_assetManager = assetManager;
            m_gfxLayer = gfxLayer;

            m_polyBuilder = new PolygonBuilder(this);
        }

        public string Name { get; private set; } = String.Empty;

        public List<Polygon> Polygons { get; private set; } = [];

        public IMeshBuilder WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(Name));

            Name = name;
            return this;
        }

        public IPolygonBuilder GetPolygonBuilder()
            => m_polyBuilder;

        public IMeshBuilder WithPolygons<T>(IEnumerable<T> polys, Action<T, IPolygonBuilder> config)
        {
            foreach (var p in polys)
            {
                config(p, m_polyBuilder);
                m_polyBuilder.Close();
            }

            return this;
        }

        private void Validate()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Name, nameof(Name));

            m_polyBuilder.Close();

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
