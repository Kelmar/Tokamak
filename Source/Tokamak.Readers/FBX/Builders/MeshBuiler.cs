using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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

        public ObjectGraph ObjectGraph => State.ObjectGraph;

        private IEnumerable<FBXMesh> ReadMeshes()
        {
            var meshNodes = State.RootNode.Children["geometry"].ToList();

            foreach (var node in meshNodes)
            {
                var mesh = new FBXMesh(State, node);
                ReadMeshDetails(mesh);

                yield return mesh;
            }
        }

        public List<FBXMesh> Meshes { get; }

        private List<int> ReadIndexData(FBXObject mesh)
        {
            return mesh.Node
                .Children["PolygonVertexIndex"]
                .SelectMany(n => n.Properties[0].AsEnumerable<int>())
                .ToList();
        }

        private List<Vector3> ReadVertexData(FBXObject mesh)
        {
            return mesh.Node
                .Children["Vertices"]
                .SelectMany(v => v.Properties[0].AsEnumerable<float>())
                .ToList() // Chunk needs the list to be realized first.
                .Chunk(3) // Group into threes
                .Select(Settings.MapToVector) // Convert to vertex
                .ToList();
        }

        public void ReadMesh(FBXObject obj)
        {
            // Pull raw data from FBX structure
            var indices = ReadIndexData(obj);
            var vectors = ReadVertexData(obj);

            var maxVert = vectors.Max(v => v.Z);

            var uvMapper = new UVMapper(obj.Node.Children["LayerElementUV"].FirstOrDefault());
            var normalMapper = new NormalMapper(Settings, obj.Node.Children["LayerElementNormal"].FirstOrDefault());
            var materialMapper = new MaterialMapper(obj.Node.Children["LayerElementMaterial"].FirstOrDefault());

            // Generate a list of polygons with flat data.
            var polygons = ToPolys(indices, vectors, uvMapper, materialMapper, normalMapper).ToList();
        }

        private void ReadMeshDetails(FBXMesh mesh)
        {
            // Pull raw data from FBX structure
            var indices = ReadIndexData(mesh);
            var vectors = ReadVertexData(mesh);

            var maxVert = vectors.Max(v => v.Z);

            var uvMapper = new UVMapper(mesh.Node.Children["LayerElementUV"].FirstOrDefault());
            var normalMapper = new NormalMapper(Settings, mesh.Node.Children["LayerElementNormal"].FirstOrDefault());
            var materialMapper = new MaterialMapper(mesh.Node.Children["LayerElementMaterial"].FirstOrDefault());

            // Generate a list of polygons with flat data.
            mesh.Polygons = ToPolys(indices, vectors, uvMapper, materialMapper, normalMapper).ToList();
        }

        /// <summary>
        /// Convert data from lists into a list of polygons.
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="vectors"></param>
        /// <param name="normals"></param>
        /// <returns></returns>
        private IEnumerable<FBXPolygon> ToPolys(
            IEnumerable<int> indices,
            List<Vector3> vectors,
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

                //if (materialIdx < m_parent.Materials.Count)
                //{
                //    var material = m_parent.Materials[materialIdx];
                //    color = material.Parameters.DiffuseColor;
                //}

                current.Vectors.Add(vectors[i]);
                normalMapper.AddNormal(current, polyIdx, indexNo, i);
                current.TexCoord.Add(uvMapper.GetUV(polyIdx, indexNo, i));

                current.Material.Add(materialIdx);

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
