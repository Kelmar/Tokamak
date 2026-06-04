using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

using Tokamak.Readers.FBX.Mappers;

namespace Tokamak.Readers.FBX.Readers
{
    /// <summary>
    /// Class for building mesh objects from FBX node.
    /// </summary>
    internal class MeshReader
    {
        public MeshReader(ReadState state)
        {
            State = state;
        }

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
                .Select(Settings.MapToVector) // Convert to vertex
                .ToList();
        }

        public MeshInfo ReadMesh(FBXObject obj)
        {
            var modelObj = State.Models.FirstOrDefault(m => obj.ParentIds.Contains(m.Id));

            // Use all available materials by default.
            var materials = State.Materials;

            if (modelObj != null)
            {
                var tmpList = new List<MaterialInfo>();

                // We need to make sure we preserve the order that we see in the model's defintion.
                foreach (var id in modelObj.MaterialIds)
                {
                    var material = materials.FirstOrDefault(m => m.Id == id);

                    if (material == null)
                    {
                        // Replace with a default material.
                        material = new MaterialInfo
                        {
                            Id = -1,
                            Name = "Default",
                        };
                    }

                    tmpList.Add(material);
                }

                materials = tmpList;
            }

            if (materials.Count == 0)
                materials = [new MaterialInfo()]; // Add at least some sort of basic material.

            // Pull raw data from FBX structure
            var indices = ReadIndexData(obj);
            var vectors = ReadVertexData(obj);

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
                p => p.SelectMany(p => p.AsEnumerable<float>()).ToList().Chunk(3).Select(Settings.MapToVector)
            );

            var materialMapper = new LayerMapper<int>(
                obj.Node.Children.FirstWithName("LayerElementMaterial"),
                "Materials",
                "MaterialIndex",
                p => p.SelectMany(p => p.AsEnumerable<int>())
            );

            // Generate a list of polygons with flat data.
            var polygons = ToPolys(indices, vectors, materials, uvMapper, materialMapper, normalMapper).ToList();

            return new MeshInfo
            {
                Id = obj.Id,
                Name = obj.Name,
                ModelId = modelObj?.Id ?? 0,
                Polygons = polygons
            };
        }

        private IEnumerable<FBXPolygon> ToPolys(
            IEnumerable<int> indices,
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
            int lastMaterialIdx = 0;
            int indexNo = 0; // Index of the index.... >_<
            int polyIdx = 0;

            foreach (var index in indices)
            {
                bool boundary = index < 0;
                int vectorIdx = boundary ? ~index : index;

                int materialIdx = materialMapper.GetItem(indexNo, polyIdx, vectorIdx);

                if (materialIdx < 0 || materialIdx >= materials.Count)
                    materialIdx = lastMaterialIdx; // Sanity, use last known good material index.

                lastMaterialIdx = materialIdx;
                var material = materials[materialIdx];

                current.Vectors.Add(vectors[vectorIdx]);
                current.Normals.Add(normalMapper.GetItem(indexNo, polyIdx, vectorIdx));
                current.TexCoord.Add(uvMapper.GetItem(indexNo, polyIdx, vectorIdx));

                current.Material.Add(material.DiffuseColor);

                if (boundary)
                {
                    //normalMapper.FinalizeNormals(current);
                    yield return current;

                    ++polyIdx;
                    current = new FBXPolygon();
                }

                ++indexNo;
            }

            if (current.Vectors.Count > 2)
            {
                //normalMapper.FinalizeNormals(current);
                yield return current;
            }
        }
    }
}
