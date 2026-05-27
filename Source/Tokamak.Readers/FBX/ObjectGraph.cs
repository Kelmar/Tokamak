using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Tokamak.Utilities;

namespace Tokamak.Readers.FBX
{
    /// <summary>
    /// Holds the mapping from the Connection/C nodes for the FBX object hierarchy.
    /// </summary>
    internal class ObjectGraph
    {
        private readonly Node m_rootNode;

        private readonly ILookup<long, long> m_objectGraph;

        private readonly ILookup<long, long> m_objectProperties;

        private readonly ILookup<long, long> m_propertyObjects;

        private readonly ILookup<long, long> m_propertyProperties;

        private readonly Dictionary<long, Node> m_objects;

        public ObjectGraph(Node rootNode)
        {
            m_rootNode = rootNode;

            var connections = GetConnections().ToList();

            m_objectGraph = connections
               .Where(c => c.Type == "OO")
               .ToLookup(c => c.To, c => c.From);

            m_objectProperties = connections
                .Where(c => c.Type == "OP")
                .ToLookup(c => c.To, c => c.From);

            m_propertyObjects = connections
                .Where(c => c.Type == "PO")
                .ToLookup(c => c.To, c => c.From);

            m_propertyProperties = connections
                .Where(c => c.Type == "PP")
                .ToLookup(c => c.To, c => c.From);

            m_objects = GetObjects();
        }

        private IEnumerable<Connection> GetConnections()
        {
            var connectionNode = m_rootNode.Children["Connections"].FirstOrDefault();

            if (connectionNode == null)
                yield break;

            foreach (var n in connectionNode.Children["C"])
            {
                Connection? c = Connection.Build(n);

                if (c != null)
                    yield return c;
            }
        }

        private (long Id, Node Node) WithIndex(Node node)
        {
            if ((node.Properties.Count < 1) || !node.Properties[0].Type.IsNumeric)
                return (-1, node);

            return (Convert.ToInt64(node.Properties[0].Data), node);
        }

        private Dictionary<long, Node> GetObjects()
        {
            var objectsNodes = m_rootNode.Children["Objects"];

            return objectsNodes
                .SelectMany(n => n.Children.Flatten())
                .Select(WithIndex)
                .Where(x => x.Id != -1)
                .ToDictionary(x => x.Id, x => x.Node);
        }

        public IEnumerable<long> GetChildObjectIds(long parentId) => m_objectGraph[parentId];

        public IEnumerable<Node> GetChildObjects(long parentId)
        {
            foreach (var id in GetChildObjectIds(parentId))
            {
                if (m_objects.TryGetValue(id, out var node))
                    yield return node;
            }
        }

        public IEnumerable<long> GetObjectPropertyIds(long objectId) => m_objectProperties[objectId];

        public IEnumerable<long> GetPropertyObjectIds(long propertyId) => m_propertyObjects[propertyId];

        public IEnumerable<long> GetChildPropertyIds(long propertyId) => m_propertyProperties[propertyId];
    }
}
