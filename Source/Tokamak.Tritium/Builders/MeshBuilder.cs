using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Assets;
using Tokamak.Mathematics;
using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Builders
{
    internal class MeshBuilder : IMeshBuilder
    {
        private class PolygonBuilder : IPolygonBuilder
        {
            private MeshBuilder m_meshBuilder;

            public PolygonBuilder(MeshBuilder meshBuilder)
            {
                m_meshBuilder = meshBuilder;
            }

            public Polygon Current { get; set; } = new();

            public IPolygonBuilder AddVertices(params IEnumerable<Vector3> vertices)
            {
                Current.Vectors.AddRange(vertices);
                return this;
            }

            public IPolygonBuilder AddNormals(params IEnumerable<Vector3> normals)
            {
                Current.Normals.AddRange(normals);
                return this;
            }

            public IPolygonBuilder AddUVs(params IEnumerable<Vector2> uvs)
            {
                Current.TexCoord.AddRange(uvs);
                return this;
            }

            public IPolygonBuilder AddColors(params IEnumerable<Color> colors)
                => AddColors(colors.Select(c => c.ToVector()));

            public IPolygonBuilder AddColors(params IEnumerable<Vector4> colors)
            {
                Current.Colors.AddRange(colors);
                return this;
            }

            public IPolygonBuilder Close()
            {
                // TODO: Fully validate polygon

                if (Current.Vectors.Count == 0)
                    return this; // Nothing to do.

                m_meshBuilder.Polygons.AddRange(Current.SplitIntoTriangles());

                Current = new();
                return this;
            }
        }

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

        public IMeshBuilder WithPolygons(IEnumerable<Polygon> polys)
        {
            Polygons = polys.SelectMany(p => p.SplitIntoTriangles()).ToList();
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
