using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Tokamak.Utilities;

using Tokamak.Readers.FBX.ObjectWrappers;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Readers.FBX
{
    public sealed class FBXReader : IDisposable
    {
        internal const string BINARY_MAGIC = "Kaydara FBX Binary  ";

        private readonly Stream m_input;
        private readonly IParser m_parser;
        private readonly bool m_closeStream;

        private readonly Encoding m_encoding;

        public FBXReader(Stream input, Encoding? encoding = null, bool closeStream = true)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));

            if (!input.CanRead)
                throw new ArgumentException("Stream not open for reading", nameof(input));

            m_input = input;
            m_closeStream = closeStream;

            // Use ASCII by default?
            m_encoding = encoding ?? Encoding.UTF8;

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
            if (m_closeStream)
            {
                m_input.Close();
                m_input.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        private string ReadString(int length)
        {
            byte[] buffer = new byte[length];

            int rd = m_input.Read(buffer);

            if (rd != buffer.Length)
                throw new Exception($"Unable to read string of length {length}");

            return m_encoding.GetString(buffer).TrimEnd('\0');
        }

        private Node GetNodes()
        {
            List<Node> children = [];

            for (;;)
            {
                Node? node = m_parser.ReadNode();

                if (node == null)
                    break;

                children.Add(node);
            }

            return new Node
            {
                Name = String.Empty,
                Properties = [],
                Children = children.ToLookup(c => c.Name, c => c),
            };
        }

        internal static T MapCompoundTo<T>(IEnumerable<CompoundProperty> fbxProps)
            where T : class, new()
        {
            Type type = typeof(T);
            var typeProps = type.GetProperties().Where(p => p.CanWrite && p.CanRead);

            T result = new();

            foreach (var typeProp in typeProps)
            {
                var notMapped = typeProp.GetCustomAttribute<NotMappedAttribute>();

                if (notMapped != null)
                    continue;

                var colAttr = typeProp.GetCustomAttribute<ColumnAttribute>();

                string name = colAttr?.Name ?? typeProp.Name;

                var fbxProp = fbxProps.FirstOrDefault(p => p.Name == name);

                if (fbxProp == null)
                    continue;

                try
                {
                    object data = Convert.ChangeType(fbxProp.Data, typeProp.PropertyType);
                    typeProp.SetValue(result, data);
                }
                catch
                {
                    // TODO: Might be worth while logging that we can't set the value.
                    continue;
                }
            }

            return result;
        }

        internal static T MapCompoundTo<T>(Node rootNode)
           where T : class, new()
        {
            Type type = typeof(T);

            var tableAttr = type.GetCustomAttribute<TableAttribute>();

            string subNode = tableAttr?.Name ?? type.Name;

            var node = rootNode.Children[subNode].First();

            var fbxProps = CompoundProperty.BuildAllFor(node);

            return MapCompoundTo<T>(fbxProps);
        }

        private (long Id, Node Node) WithIndex(Node node)
        {
            if ((node.Properties.Count < 1) || !node.Properties[0].Type.IsNumeric)
                return (-1, node);

            return (Convert.ToInt64(node.Properties[0].Data), node);
        }

        public IEnumerable<Mesh> Import()
        {
            Node dataRoot = GetNodes();

            var settings = MapCompoundTo<GlobalSettings>(dataRoot);

            var objectGraph = new ObjectGraph(dataRoot);

            var objectsNodes = dataRoot.Children["Objects"];

            var objectNodes = objectsNodes
                .SelectMany(n => n.Children.Flatten())
                .Select(WithIndex)
                .Where(x => x.Id != -1)
                .ToDictionary(x => x.Id, x => x.Node);

            var rootIds = objectGraph.GetChildObjectIds(0);

            // At this point we should have a valid node structure, but we still need to recreate the object hierarchy.

            // Start with the root models
            var rootModels = objectNodes
                .Where(kvp => rootIds.Contains(kvp.Key) && kvp.Value.Name.ToLower() == "model")
                .Select(kvp => new ModelBuilder(kvp.Value))
                .ToList();

            // Split out our objects into models, geometry and materials as flat lists.

            //var models = objectsNodes
            //    .SelectMany(o => o.Children["Model"])
            //    .Select(n => new ModelBuilder(n))
            //    .ToDictionary(o => o.ID, o => o);

            var mats = objectsNodes
                .SelectMany(o => o.Children["Material"])
                .Select(n => new MaterialBuilder(n))
                .ToArray();

            var geos = objectsNodes
                .SelectMany(o => o.Children["Geometry"])
                .Select(n => new MeshBuilder(settings, mats, n))
                .ToList();

            return geos.Select(g => g.Mesh);
        }
    }
}
