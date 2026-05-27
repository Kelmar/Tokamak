using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Readers.FBX.Builders
{
    /// <summary>
    /// Class for building mesh objects from FBX node.
    /// </summary>
    internal class MeshBuilder : FBXObject
    {
        private readonly ModelBuilder m_parent;

        public MeshBuilder(ModelBuilder parent, Node node)
            : base(parent, node)
        {
            m_parent = parent;

            Mesh = new Mesh();

            ReadMeshDetails();
        }

        /// <summary>
        /// Engine mesh object that was built up from this node's data.
        /// </summary>
        public Mesh Mesh { get; }

        private List<int> ReadIndexData()
        {
            return Node
                .Children["PolygonVertexIndex"]
                .SelectMany(n => n.Properties[0].AsEnumerable<int>())
                .ToList();
        }

        private List<Vector3> ReadVertexData()
        {
            return Node
                .Children["Vertices"]
                .SelectMany(v => v.Properties[0].AsEnumerable<float>())
                .ToList() // Chunk needs the list to be realized first.
                .Chunk(3) // Group into threes
                .Select(Settings.MapToVector) // Convert to vertex
                .ToList();
        }

        private void ReadMeshDetails()
        {
            // Pull raw data from FBX structure
            var indices = ReadIndexData();
            var vectors = ReadVertexData();

            var maxVert = vectors.Max(v => v.Z);

            var uvMapper = new UVMapper(Node.Children["LayerElementUV"].FirstOrDefault());
            var normalMapper = new NormalMapper(Settings, Node.Children["LayerElementNormal"].FirstOrDefault());
            var materialMapper = new MaterialMapper(Node.Children["LayerElementMaterial"].FirstOrDefault());

            // Generate a list of polygons with flat data.
            Mesh.Polygons = ToPolys(indices, vectors, uvMapper, materialMapper, normalMapper).ToList();
        }

        /// <summary>
        /// Convert data from lists into a list of polygons.
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="vectors"></param>
        /// <param name="normals"></param>
        /// <returns></returns>
        private IEnumerable<Polygon> ToPolys(
            IEnumerable<int> indices,
            List<Vector3> vectors,
            UVMapper uvMapper, 
            MaterialMapper materialMapper,
            NormalMapper normalMapper)
        {
            // FBX uses a negative number to indicate the end of a polygon.
            // Note that the negative number is a bitwise negation of the last index
            // In this way zero is represented as -1

            var current = new Polygon();
            int indexNo = 0; // Index of the index.... >_<
            int polyIdx = 0;

            foreach (var index in indices)
            {
                bool boundary = index < 0;
                int i = boundary ? ~index : index;

                Vector4 color = Vector4.One;

                var materialIdx = materialMapper.GetMaterial(polyIdx, indexNo, i);

                if (materialIdx < m_parent.Materials.Count)
                {
                    var material = m_parent.Materials[materialIdx];
                    color = material.Parameters.DiffuseColor;
                }

                current.Vectors.Add(vectors[i]);
                current.Colors.Add(color);
                current.TexCoord.Add(uvMapper.GetUV(polyIdx, indexNo, i));

                normalMapper.AddNormal(current, polyIdx, indexNo, i);

                if (boundary)
                {
                    normalMapper.FinalizeNormals(current);
                    yield return current;

                    ++polyIdx;
                    current = new Polygon();
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
