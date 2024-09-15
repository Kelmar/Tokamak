using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Tokamak.Mathematics;

using Tokamak.Tritium.Buffers;

using Tokamak.Readers.FBX.ObjectWrappers;
using System.Numerics;

namespace Tokamak.Readers.FBX
{
    public class FBXReader : IDisposable
    {
        private const string BINARY_MAGIC = "Kaydara FBX Binary  ";

        private readonly Stream m_input;
        private readonly IParser m_parser;

        public FBXReader(Stream input)
        {
            if (!input.CanRead)
                throw new ArgumentException("Stream not open for reading", nameof(input));

            m_input = input;

            m_input.Seek(0, SeekOrigin.Begin);

            string magic = ReadString(21);

            if (magic == BINARY_MAGIC)
            {
                // Create a binary parser
                m_parser = new BinaryFormatReader(m_input);
            }
            else
            {
                // Create a text parser
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            m_input.Dispose();
        }

        private string ReadString(int length)
        {
            byte[] buffer = new byte[length];

            int rd = m_input.Read(buffer);

            if (rd != buffer.Length)
                throw new Exception($"Unable to read string of length {length}");

            return Encoding.UTF8.GetString(buffer).TrimEnd('\0');
        }

        internal Node GetNodes()
        {
            var root = new Node();

            for (;;)
            {
                Node node = m_parser.ReadNode();

                if (node == null)
                    break;

                root.AddChild(node);
            }

            return root;
        }

        public IEnumerable<Mesh> Import()
        {
            Node dataRoot = GetNodes();

            // At this point we should have a valid node structure, but we still need to recreate the object hierarchy.

            var dataObjs = dataRoot.GetChildren("Objects");

            // Split out our objects into models, geometry and materials as flat lists.

            var models = dataObjs
                .SelectMany(o => o.GetChildren("Model"))
                .Select(ReadObject)
                .ToList();

            var geos = dataObjs
                .SelectMany(o => o.GetChildren("Geometry"))
                .Select(ReadMesh)
                .ToList();

            var mats = dataObjs
                .SelectMany(o => o.GetChildren("Material"))
                .Select(ReadMaterial)
                .ToList();

            // Get all of our connections as a flat list.
            var connects = dataRoot
                .GetChildren("Connections")
                .SelectMany(c => c.GetChildren("C"))
                .ToList();

            return geos.Select(g => g.Mesh);
        }

        private MeshWrapper ReadMesh(Node mesh)
        {
            var rval = new MeshWrapper
            {
                Node = mesh,
                ID = mesh.Properties[0].AsInt(),
                Name = mesh.Properties[1].AsString(),
                Mesh = new Mesh()
            };

            // Pull raw data from FBX structure
            var vectors = mesh
                .GetChildren("Vertices")
                .SelectMany(v => v.Properties[0].AsEnumerable<float>())
                .ToList() // Chunk needs the list to be realized first.
                .Chunk(3) // Group into threes
                .Select(MathX.ToVector3) // Convert to vertex
                .ToList();

            var normals = mesh
                .GetChildren("LayerElementNormal")
                .SelectMany(n => n.GetChildren("Normals"))
                .SelectMany(n => n.Properties[0].AsEnumerable<float>())
                .ToList()
                .Chunk(3)
                .Select(MathX.ToVector3)
                .ToList();

            var indices = mesh
                .GetChildren("PolygonVertexIndex")
                .Select(n => n.Properties[0])
                .SelectMany(p => p.AsEnumerable<int>());

            //var texCoords = mesh
            //    .GetChildren("LayerElementUV")
            //    .Select(n => n.Properties[0])
            //    .SelectMany(p => p.AsEnumerable<float>())
            //    .ToList()
            //    .Chunk(2)
            //    .Select(MathX.ToVector2)
            //    .ToList();

            //var mats = mesh.GetChildren("LayerElementMaterial");

            // Generate a list of polygons with normals.
            rval.Mesh.Polygons = ToPolys(indices, vectors, normals).ToList();

            return rval;
        }

        private IEnumerable<Polygon> ToPolys(
            IEnumerable<int> indices,
            List<Vector3> vectors,
            List<Vector3> normals)
        {
            // FBX uses a negative number to indicate the end of a polygon.
            // Note that the negative number is a bitwise negation of the last index
            // In this way zero is represented as -1

            var lastPoly = new Polygon();
            int indexNo = 0; // Index of the index.... >_<

            foreach (var index in indices)
            {
                lastPoly.Normals.Add(normals[indexNo]);

                if (index < 0)
                {
                    int i = ~index;

                    lastPoly.Vectors.Add(vectors[i]);
                    lastPoly.TexCoord.Add(Vector2.Zero); // texCoords[i]);

                    yield return lastPoly;

                    lastPoly = new Polygon();
                }
                else
                {
                    lastPoly.Vectors.Add(vectors[index]);
                    lastPoly.TexCoord.Add(Vector2.Zero); //texCoords[index]);
                }

                ++indexNo;
            }

            if (lastPoly.Vectors.Count > 2)
                yield return lastPoly;
        }

        private IFBXObject ReadObject(Node model)
        {
            var rval = new ObjectWrapper
            {
                Node = model,
                ID = model.Properties[0].AsInt(),
                Name = model.Properties[1].AsString()
            };

            return rval;
        }

        private IFBXObject ReadMaterial(Node material)
        {
            var rval = new MaterialWrapper
            {
                Node = material,
                ID = material.Properties[0].AsInt(),
                Name = material.Properties[1].AsString()
            };

            return rval;
        }
    }
}
