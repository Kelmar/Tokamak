using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Import.FBX.DOM;
using Tokamak.Import.FBX.Mappers;

namespace Tokamak.Import.FBX.Readers
{
    /// <summary>
    /// Class for building mesh objects from FBX node.
    /// </summary>
    internal class MeshReader : IFBXObjectReader
    {
        private readonly ReadState m_state;
        private readonly FBXObject m_fbxObject;

        private readonly LayerMapper<Vector2> m_uvMapper;
        private readonly LayerMapper<Vector3> m_normalMapper;
        private readonly LayerMapper<int> m_materialMapper;

        public MeshReader(ReadState state, FBXObject obj)
        {
            m_state = state;
            m_fbxObject = obj;

            m_uvMapper = new(m_fbxObject.Node.Children, "LayerElementUV", "UV", "UVIndex", MapUVs);
            m_normalMapper = new(m_fbxObject.Node.Children, "LayerElementNormal", "Normals", "NormalsIndex", MapNormals);
            m_materialMapper = new(m_fbxObject.Node.Children, "LayerElementMaterial", "Materials", "MaterialIndex", MapMaterials);
        }

        public GlobalSettings Settings => m_state.Settings;

        private List<int> ReadIndexData()
        {
            return m_fbxObject.Node.Children
                .WithName("PolygonVertexIndex")
                .SelectMany(n => n.Properties[0].AsEnumerable<int>())
                .ToList();
        }

        private List<Vector3> ReadVertexData()
        {
            return m_fbxObject.Node.Children
                .WithName("Vertices")
                .SelectMany(v => v.Properties[0].AsVector3s())
                .Select(Settings.SwizzleAxes)
                .ToList();
        }

        private IEnumerable<Vector2> MapUVs(IEnumerable<NodeProperty> properties)
            => properties.SelectMany(p => p.AsVector2s());

        private IEnumerable<Vector3> MapNormals(IEnumerable<NodeProperty> properties)
            => properties.SelectMany(p => p.AsVector3s()).Select(Settings.SwizzleAxes);

        private IEnumerable<int> MapMaterials(IEnumerable<NodeProperty> properties)
            => properties.SelectMany(p => p.AsEnumerable<int>());

        public void Process()
        {
            var sceneObj = m_state.SceneObjects.FirstOrDefault(m => m_fbxObject.ParentIds.Contains(m.Id));

            // Pull raw data from FBX structure
            var indices = ReadIndexData();
            var vectors = ReadVertexData();

            if (indices.Count == 0 || vectors.Count == 0)
                throw new Exception("Mesh with no indices or vectors in FBX file.");

            int lastVectorIndex = 0;

            var vertices = new Dictionary<int, VertexInfo>(vectors.Count);
            var polygons = new List<FBXPolygon>(indices.Count(i => i < 0));

            var currentPoly = new FBXPolygon
            {
                Index = polygons.Count
            };

            for (int indexNumber = 0; indexNumber < indices.Count; ++indexNumber)
            {
                int index = indices[indexNumber];

                // FBX uses a negative number to indicate the end of a polygon.
                // Note that the negative number is a bitwise negation of the last index
                // In this way zero is represented as -1

                bool boundary = index < 0;
                int vectorIndex = boundary ? ~index : index;

                int materialIndex = m_materialMapper.GetItem(indexNumber, currentPoly.Index, vectorIndex);

                if (vectorIndex >= vectors.Count)
                {
                    // Sanity, use last known good vector index.
                    // Wondering if it makes more sense to throw out this mesh and report it as corrupt.
                    vectorIndex = lastVectorIndex;
                }

                lastVectorIndex = vectorIndex;

                if (!vertices.TryGetValue(vectorIndex, out VertexInfo? vertex))
                {
                    vertex = new VertexInfo
                    {
                        Index = vectorIndex,
                        Vertex = vectors[vectorIndex],
                        Normal = m_normalMapper.GetItem(indexNumber, currentPoly.Index, vectorIndex),
                        TexCoord = m_uvMapper.GetItem(indexNumber, currentPoly.Index, vectorIndex),
                        MaterialIndex = materialIndex
                    };

                    vertices[vectorIndex] = vertex;
                }

                currentPoly.Vertices.Add(vectorIndex);

                if (boundary)
                {
                    // TODO: Perform sanity check that poly has at least 3 verts.

                    polygons.Add(currentPoly);

                    currentPoly = new FBXPolygon
                    {
                        Index = polygons.Count
                    };
                }
            }

            if (currentPoly.Vertices.Count > 2)
            {
                //normalMapper.FinalizeNormals(current);
                polygons.Add(currentPoly);
            }

            // TODO: Perform sanity check on discovered verts.

            var meshInfo = new MeshInfo
            {
                Id = m_fbxObject.Id,
                Name = m_fbxObject.Name,
                ModelId = sceneObj?.Id ?? 0,
                Polygons = polygons,
                Vertices = vertices.Values.ToList()
            };

            m_state.Meshes.Add(meshInfo);
        }
    }
}
