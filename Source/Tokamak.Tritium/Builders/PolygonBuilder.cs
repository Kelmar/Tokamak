using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Import.Builders;

using Tokamak.Mathematics;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Builders
{
    internal class PolygonBuilder(MeshBuilder meshBuilder) : IPolygonBuilder
    {
        private MeshBuilder m_meshBuilder = meshBuilder;

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
}
