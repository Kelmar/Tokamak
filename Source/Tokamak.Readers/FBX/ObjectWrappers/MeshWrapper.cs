using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Readers.FBX.ObjectWrappers
{
    /// <summary>
    /// Wrapper around a mesh object for loading FBX files.
    /// </summary>
    internal class MeshWrapper : IFBXObject
    {
        public MeshWrapper(GlobalSettings settings, Node node)
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

        private void ReadMeshDetails()
        {
            // Pull raw data from FBX structure
            var vectors = Node
                .GetChildren("Vertices")
                .SelectMany(v => v.Properties[0].AsEnumerable<float>())
                .ToList() // Chunk needs the list to be realized first.
                .Chunk(3) // Group into threes
                .Select(Settings.MapToVector) // Convert to vertex
                .ToList();

            var normalMapper = new NormalMapper(Settings, Node.GetChildren("LayerElementNormal")?.FirstOrDefault());

            var indices = Node
                .GetChildren("PolygonVertexIndex")
                .SelectMany(n => n.Properties[0].AsEnumerable<int>())
                .ToList();

            //var texCoords = Node
            //    .GetChildren("LayerElementUV")
            //    .Select(n => n.Properties[0])
            //    .SelectMany(p => p.AsEnumerable<float>())
            //    .ToList()
            //    .Chunk(2)
            //    .Select(MathX.ToVector2)
            //    .ToList();

            //var mats = Node.GetChildren("LayerElementMaterial");

            // Generate a list of polygons with flat data.
            Mesh.Polygons = ToPolys(indices, vectors, normalMapper).ToList();
        }

        /// <summary>
        /// Convert data from lists into a list of polygons.
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="vectors"></param>
        /// <param name="normals"></param>
        /// <returns></returns>
        private IEnumerable<Polygon> ToPolys(IEnumerable<int> indices, List<Vector3> vectors, NormalMapper normalMapper)
        {
            // FBX uses a negative number to indicate the end of a polygon.
            // Note that the negative number is a bitwise negation of the last index
            // In this way zero is represented as -1

            var current = new Polygon();
            int indexNo = 0; // Index of the index.... >_<

            foreach (var index in indices)
            {
                if (index < 0)
                {
                    int i = ~index;

                    normalMapper.AddNormal(current, indexNo, i);
                    current.Vectors.Add(vectors[i]);
                    current.TexCoord.Add(Vector2.Zero); // texCoords[i]);

                    normalMapper.FinalizeNormals(current);
                    yield return current;

                    current = new Polygon();
                }
                else
                {
                    normalMapper.AddNormal(current, indexNo, index);
                    current.Vectors.Add(vectors[index]);
                    current.TexCoord.Add(Vector2.Zero); //texCoords[index]);
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
