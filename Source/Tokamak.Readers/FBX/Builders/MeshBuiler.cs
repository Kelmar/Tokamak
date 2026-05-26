using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Readers.FBX.Builders;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Readers.FBX.ObjectWrappers
{
    /// <summary>
    /// Class for building mesh objects from FBX node.
    /// </summary>
    internal class MeshBuilder : IFBXObject
    {
        public MeshBuilder(GlobalSettings settings, Node node)
        {
            Node = node;
            ID = Node.Properties[0].AsInt();
            Name = Node.Properties[1].AsString();

            Mesh = new Mesh();
            Settings = settings;

            ReadMeshDetails();
        }

        /// <summary>
        /// The unique ID of this object in the file.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// The name of this mesh in the file.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The FBX node we read from.
        /// </summary>
        public Node Node { get; }

        /// <summary>
        /// Engine mesh object that was built up from this node's data.
        /// </summary>
        public Mesh Mesh { get; }

        public GlobalSettings Settings { get; }

        private List<int> ReadIndexData()
        {
            return Node
                .GetChildren("PolygonVertexIndex")
                .SelectMany(n => n.Properties[0].AsEnumerable<int>())
                .ToList();
        }

        private List<Vector3> ReadVertexData()
        {
            return Node
                .GetChildren("Vertices")
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

            var uvMapper = new UVMapper(Node.GetChildren("LayerElementUV")?.FirstOrDefault());
            var normalMapper = new NormalMapper(Settings, Node.GetChildren("LayerElementNormal")?.FirstOrDefault());

            //var mats = Node.GetChildren("LayerElementMaterial");

            // Generate a list of polygons with flat data.
            Mesh.Polygons = ToPolys(indices, vectors, uvMapper, normalMapper).ToList();
        }

        /// <summary>
        /// Convert data from lists into a list of polygons.
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="vectors"></param>
        /// <param name="normals"></param>
        /// <returns></returns>
        private IEnumerable<Polygon> ToPolys(IEnumerable<int> indices, List<Vector3> vectors, UVMapper uvMapper, NormalMapper normalMapper)
        {
            // FBX uses a negative number to indicate the end of a polygon.
            // Note that the negative number is a bitwise negation of the last index
            // In this way zero is represented as -1

            var current = new Polygon();
            int indexNo = 0; // Index of the index.... >_<

            foreach (var index in indices)
            {
                bool boundary = index < 0;
                int i = boundary ? ~index : index;

                current.Vectors.Add(vectors[i]);
                current.TexCoord.Add(uvMapper.GetUV(indexNo, i));

                normalMapper.AddNormal(current, indexNo, i);

                if (boundary)
                {
                    normalMapper.FinalizeNormals(current);
                    yield return current;

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
