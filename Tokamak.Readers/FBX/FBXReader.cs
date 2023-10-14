using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Tokamak.Buffer;
using Tokamak.Mathematics;
using Tokamak.Readers.FBX.ObjectWrappers;

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

            // At this point we should have a valid node structure, but we still need to recreate the object heirarchy.

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

            var verts = mesh.GetChildren("Vertices");
            var indices = mesh.GetChildren("PolygonVertexIndex");

            rval.Mesh.Verts = verts
                .Select(v => v.Properties[0])
                .SelectMany(p => p.AsEnumerable<float>())
                .ToList() // Chunk needs the list to be realized first.
                .Chunk(3)
                .Select(v => v.ToVector3())
                .ToList();

            /*
            foreach (var vertNode in verts)
            {
                var props = vertNode.Properties[0];
                var values = props.AsEnumerable<float>();
                var grps = values.ToList().Chunk(3);
                rval.Mesh.Verts = grps.Select(v => v.ToVector3()).ToList();
            }
            */

            var indexValues = indices
                .Select(v => v.Properties[0])
                .SelectMany(p => p.AsEnumerable<int>());

            rval.Mesh.Indicies = ToPolys(indexValues)
                .SelectMany(p => p.SplitIntoTriangles())
                .SelectMany(p => p.Indices)
                .ToList();

            /*
            var normals = mesh
                .GetChildren("LayerElementNormal")
                .SelectMany(n => n.GetChildren("Normals"));

            rval.Mesh.Normals = normals
                .Select(n => n.Properties[0])
                .SelectMany(p => p.AsEnumerable<float>())
                .Chunk(3)
                .Select(v => v.ToVector3())
                .ToList();
            */
            /*
            var uvs = mesh.GetChildren("LayerElementUV");
            var mats = mesh.GetChildren("LayerElementMaterial");
            */

            return rval;
        }

        private IEnumerable<Polygon> ToPolys(IEnumerable<int> values)
        {
            // FBX uses a negative number to indicate the end of a polygon.

            var lastPoly = new Polygon();

            foreach (var v in values)
            {
                if (v < 0)
                {
                    lastPoly.Indices.Add((uint)-v);
                    yield return lastPoly;

                    lastPoly = new Polygon();
                }
                else
                    lastPoly.Indices.Add((uint)v);
            }

            if (lastPoly.Indices.Any())
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
