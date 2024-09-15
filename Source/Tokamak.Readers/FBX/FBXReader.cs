using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

using Tokamak.Mathematics;

using Tokamak.Tritium.Geometry;

using Tokamak.Readers.FBX.ObjectWrappers;

namespace Tokamak.Readers.FBX
{
    public class FBXReader : IDisposable
    {
        private const string BINARY_MAGIC = "Kaydara FBX Binary  ";

        private readonly Stream m_input;
        private readonly IParser m_parser;

        private readonly Encoding m_encoding;

        public FBXReader(Stream input, Encoding encoding = null)
        {
            ArgumentNullException.ThrowIfNull(input);

            if (!input.CanRead)
                throw new ArgumentException("Stream not open for reading", nameof(input));

            // Use ASCII by default?
            m_encoding = encoding ?? Encoding.UTF8;

            m_input = input;

            m_input.Seek(0, SeekOrigin.Begin);

            string magic = ReadString(21);

            if (magic == BINARY_MAGIC)
                m_parser = new BinaryFormatReader(m_input, m_encoding);
            else
            {
                // TODO: Create a text parser
                throw new NotImplementedException("Text based parser for FBX not written yet.");
            }
        }

        public FBXReader(string filename)
            : this(File.OpenRead(filename))
        {
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

            return m_encoding.GetString(buffer).TrimEnd('\0');
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

            var dataObjects = dataRoot.GetChildren("Objects");

            // Split out our objects into models, geometry and materials as flat lists.

            var models = dataObjects
                .SelectMany(o => o.GetChildren("Model"))
                .Select(n => new ObjectWrapper(n))
                .ToList();

            var geos = dataObjects
                .SelectMany(o => o.GetChildren("Geometry"))
                .Select(n => new MeshWrapper(n))
                .ToList();

            var mats = dataObjects
                .SelectMany(o => o.GetChildren("Material"))
                .Select(n => new MaterialWrapper(n))
                .ToList();

            // Get all of our connections as a flat list.
            var connects = dataRoot
                .GetChildren("Connections")
                .SelectMany(c => c.GetChildren("C"))
                .ToList();

            return geos.Select(g => g.Mesh);
        }
    }
}
