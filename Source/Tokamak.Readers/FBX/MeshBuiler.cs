using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Readers.FBX.Mappers;
using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Builders
{
    /// <summary>
    /// Class for building mesh objects from FBX node.
    /// </summary>
    internal class MeshBuilder
    {
        public MeshBuilder(ReadState state)
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
            var modelObj = State.Models.Where(m => obj.ParentIds.Contains(m.Id)).FirstOrDefault();
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

            // Pull raw data from FBX structure
            var indices = ReadIndexData(obj);
            var vectors = ReadVertexData(obj);

            var uvNodes = obj.Node.Children.WithName("LayerElementUV");
            var normalNodes = obj.Node.Children.WithName("LayerElementNormal");
            var materialNodes = obj.Node.Children.WithName("LayerElementMaterial");

            var uvMapper = new UVMapper(uvNodes.FirstOrDefault());
            var normalMapper = new NormalMapper(Settings, normalNodes.FirstOrDefault());
            var materialMapper = new MaterialMapper(materialNodes.FirstOrDefault());

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
            UVMapper uvMapper, 
            MaterialMapper materialMapper,
            NormalMapper normalMapper)
        {
            // FBX uses a negative number to indicate the end of a polygon.
            // Note that the negative number is a bitwise negation of the last index
            // In this way zero is represented as -1

            var current = new FBXPolygon();
            int indexNo = 0; // Index of the index.... >_<
            int polyIdx = 0;

            foreach (var index in indices)
            {
                bool boundary = index < 0;
                int i = boundary ? ~index : index;

                var materialIdx = materialMapper.GetMaterial(polyIdx, indexNo, i);
                var material = materials[materialIdx];

                //if (materialIdx < m_parent.Materials.Count)
                //{
                //    var material = m_parent.Materials[materialIdx];
                //    color = material.Parameters.DiffuseColor;
                //}

                current.Vectors.Add(vectors[i]);
                normalMapper.AddNormal(current, polyIdx, indexNo, i);
                current.TexCoord.Add(uvMapper.GetUV(polyIdx, indexNo, i));

                current.Material.Add(material.DiffuseColor);

                if (boundary)
                {
                    normalMapper.FinalizeNormals(current);
                    yield return current;

                    ++polyIdx;
                    current = new FBXPolygon();
                }

                ++indexNo;
            }

            if (current.Vectors.Count > 2)
            {
                normalMapper.FinalizeNormals(current);
                yield return current;
            }
        }
    }
}
