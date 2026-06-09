using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

using Tokamak.Mathematics;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Mappers;

namespace Tokamak.Readers.FBX.Readers
{
    /// <summary>
    /// Class for building mesh objects from FBX node.
    /// </summary>
    internal class MeshReader : IFBXObjectReader
    {
        private int m_importCount = 0;

        public MeshReader(ReadState state)
        {
            State = state;
        }

        public string ObjectType => "geometry";

        public ReadState State { get; }

        public GlobalSettings Settings => State.Settings;

        private List<int> ReadIndexData(FBXObject mesh)
        {
            return mesh.Node.Children
                .WithName("PolygonVertexIndex")
                .SelectMany(n => n.Properties[0].AsEnumerable<int>())
                .ToList();
        }

        private List<Vector3> ReadVertexData(FBXObject mesh)
        {
            return mesh.Node.Children
                .WithName("Vertices")
                .SelectMany(v => v.Properties[0].AsEnumerable<float>())
                .ToList() // Chunk needs the list to be realized first.
                .Chunk(3) // Group into threes
                .Select(VectorEx.ToVector3) // Convert to vertex
                .Select(Settings.SwizzleAxes) // Swizzle axes
                .ToList();
        }

        private string GetAssetName(FBXObject mesh, SceneObjectInfo? sceneObj)
        {
            string name = mesh.Name;

            if (String.IsNullOrWhiteSpace(mesh.Name))
            {
                var nameBuilder = new StringBuilder();

                if (!String.IsNullOrWhiteSpace(sceneObj?.Name))
                    nameBuilder.Append(sceneObj.Name);
                else
                    nameBuilder.Append(State.FileName);

                nameBuilder.Append("_mesh_");
                nameBuilder.Append(m_importCount);

                name = nameBuilder.ToString();
            }

            return name;
        }

        public void ReadObject(FBXObject obj)
        {
            var sceneObj = State.SceneObjects.FirstOrDefault(m => obj.ParentIds.Contains(m.Id));

            // Use all available materials by default.
            var materials = State.Materials;

            if (sceneObj != null)
            {
                var tmpList = new List<MaterialInfo>();

                // We need to make sure we preserve the order that we see in the model's definition.
                foreach (var id in sceneObj.MaterialIds)
                {
                    var material = materials.FirstOrDefault(m => m.Id == id);

                    // Replace with a default material.
                    material ??= new MaterialInfo
                    {
                        Id = -1,
                        Name = "Default",
                    };

                    tmpList.Add(material);
                }

                materials = tmpList;
            }

            if (materials.Count == 0)
                materials = [new MaterialInfo()]; // Add at least some sort of basic material.

            // Pull raw data from FBX structure
            var indices = ReadIndexData(obj);
            var vectors = ReadVertexData(obj);

            if (indices.Count == 0 || vectors.Count == 0)
                throw new Exception("Mesh with no indices or vectors in FBX file.");

            var uvMapper = new LayerMapper<Vector2>(
                obj.Node.Children.FirstWithName("LayerElementUV"),
                "UV",
                "UVIndex",
                p => p.SelectMany(p => p.AsEnumerable<float>()).ToList().Chunk(2).Select(VectorEx.ToVector2)
            );

            var normalMapper = new LayerMapper<Vector3>(
                obj.Node.Children.FirstWithName("LayerElementNormal"),
                "Normals",
                "NormalsIndex",
                p => p
                    .SelectMany(p => p.AsEnumerable<float>())
                    .ToList()
                    .Chunk(3)
                    .Select(VectorEx.ToVector3) // To vector
                    .Select(Settings.SwizzleAxes) // Swizzle axes
            );

            var materialMapper = new LayerMapper<int>(
                obj.Node.Children.FirstWithName("LayerElementMaterial"),
                "Materials",
                "MaterialIndex",
                p => p.SelectMany(p => p.AsEnumerable<int>())
            );

            // Generate a list of polygons with flat data.
            var polygons = ToPolys(indices, vectors, materials, uvMapper, materialMapper, normalMapper).ToList();

            ++m_importCount;

            var meshInfo = new MeshInfo
            {
                Id = obj.Id,
                Name = GetAssetName(obj, sceneObj),
                ModelId = sceneObj?.Id ?? 0,
                Polygons = polygons
            };

            State.Meshes.Add(meshInfo);
        }

        private static IEnumerable<FBXPolygon> ToPolys(
            List<int> indices,
            List<Vector3> vectors,
            List<MaterialInfo> materials,
            LayerMapper<Vector2> uvMapper, 
            LayerMapper<int> materialMapper,
            LayerMapper<Vector3> normalMapper)
        {
            // FBX uses a negative number to indicate the end of a polygon.
            // Note that the negative number is a bitwise negation of the last index
            // In this way zero is represented as -1

            var current = new FBXPolygon();

            int lastMaterialIndex = 0;
            int lastVectorIndex = 0;

            int indexNumber = 0; // Index of the index.... >_<
            int polyIndex = 0;

            foreach (var index in indices)
            {
                bool boundary = index < 0;
                int vectorIndex = boundary ? ~index : index;

                int materialIndex = materialMapper.GetItem(indexNumber, polyIndex, vectorIndex);

                if (materialIndex < 0 || materialIndex >= materials.Count)
                    materialIndex = lastMaterialIndex; // Sanity, use last known good material index.

                if (vectorIndex >= vectors.Count)
                    vectorIndex = lastVectorIndex; // Sanity, use last known good vector index.

                lastMaterialIndex = materialIndex;
                lastVectorIndex = vectorIndex;

                var material = materials[materialIndex];

                current.Vectors.Add(vectors[vectorIndex]);
                current.Normals.Add(normalMapper.GetItem(indexNumber, polyIndex, vectorIndex));
                current.TexCoord.Add(uvMapper.GetItem(indexNumber, polyIndex, vectorIndex));

                current.Material.Add(material.DiffuseColor);

                if (boundary)
                {
                    //normalMapper.FinalizeNormals(current);
                    yield return current;

                    ++polyIndex;
                    current = new FBXPolygon();
                }

                ++indexNumber;
            }

            if (current.Vectors.Count > 2)
            {
                //normalMapper.FinalizeNormals(current);
                yield return current;
            }
        }
    }
}
