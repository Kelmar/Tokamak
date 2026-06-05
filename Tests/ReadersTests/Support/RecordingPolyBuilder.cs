using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using Tokamak.Assets;
using Tokamak.Mathematics;

namespace ReadersTests.Support
{
    internal class RecordingPolyBuilder : IPolygonBuilder
    {
        private readonly RecordingMeshBuilder m_meshBuilder;

        public RecordingPolyBuilder(RecordingMeshBuilder meshBuilder)
        {
            m_meshBuilder = meshBuilder;
        }

        public IPolygonBuilder AddColors(params IEnumerable<Color> colors)
        {
            return this;
        }

        public IPolygonBuilder AddColors(params IEnumerable<Vector4> colors)
        {
            return this;
        }

        public IPolygonBuilder AddNormals(params IEnumerable<Vector3> normals)
        {
            return this;
        }

        public IPolygonBuilder AddUVs(params IEnumerable<Vector2> uvs)
        {
            return this;
        }

        public IPolygonBuilder AddVertices(params IEnumerable<Vector3> vertices)
        {
            return this;
        }

        public IPolygonBuilder Close()
        {
            return this;
        }
    }
}
