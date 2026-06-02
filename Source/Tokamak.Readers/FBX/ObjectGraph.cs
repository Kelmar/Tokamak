using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Tokamak.Readers.FBX.Builders;
using Tokamak.Readers.FBX.DOM;
using Tokamak.Utilities;

namespace Tokamak.Readers.FBX
{
    /// <summary>
    /// Holds the mapping from the Connection/C nodes for the FBX object hierarchy.
    /// </summary>
    internal class ObjectGraph
    {
        private readonly Node m_rootNode;

        /// <summary>
        /// Lookup from child to parents.
        /// </summary>
        private readonly ILookup<long, long> m_parentGraph;

        /// <summary>
        /// Lookup from parent down to children.
        /// </summary>
        private readonly ILookup<long, long> m_objectGraph;

        private readonly ILookup<long, long> m_objectProperties;

        private readonly ILookup<long, long> m_propertyObjects;

        private readonly ILookup<long, long> m_propertyProperties;

        public ObjectGraph(ReadState state, Node rootNode)
        {
            m_rootNode = rootNode;

            var connections = GetConnections().ToList();

            var objectToObject = connections
               .Where(c => c.Type == "OO");

            m_parentGraph = objectToObject.ToLookup(c => c.From, c => c.To);
            m_objectGraph = objectToObject.ToLookup(c => c.To, c => c.From);

            m_objectProperties = connections
                .Where(c => c.Type == "OP")
                .ToLookup(c => c.To, c => c.From);

            m_propertyObjects = connections
                .Where(c => c.Type == "PO")
                .ToLookup(c => c.To, c => c.From);

            m_propertyProperties = connections
                .Where(c => c.Type == "PP")
                .ToLookup(c => c.To, c => c.From);

            Objects = GetObjects(state);
        }

        public Dictionary<long, FBXObject> Objects { get; }

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

        /// <summary>
        /// Get all nodes as a dictionary where they can be looked up by ID.
        /// </summary>
        /// <returns></returns>
        private Dictionary<long, FBXObject> GetObjects(ReadState state)
        {
            var objectNodes = m_rootNode.Children["objects"];

            return objectNodes
                .Select(n => new FBXObject(state, n))
                .ToDictionary(x => x.Id, x => x);
        }

        public IEnumerable<FBXObject> GetObjectsOfType(string type)
            => Objects.Values.WithFBXType(type);

        public IEnumerable<long> GetParentIds(long childId) => m_parentGraph[childId];

        public IEnumerable<Node> GetParentObjects(long childId)
        {
            foreach (var id in GetParentIds(childId))
            {
                if (Objects.TryGetValue(id, out var obj))
                    yield return obj.Node;
            }
        }

        public IEnumerable<long> GetChildObjectIds(long parentId) => m_objectGraph[parentId];

        public IEnumerable<Node> GetChildObjects(long parentId)
        {
            foreach (var id in GetChildObjectIds(parentId))
            {
                if (Objects.TryGetValue(id, out var obj))
                    yield return obj.Node;
            }
        }

        public IEnumerable<long> GetObjectPropertyIds(long objectId) => m_objectProperties[objectId];

        public IEnumerable<long> GetPropertyObjectIds(long propertyId) => m_propertyObjects[propertyId];

        public IEnumerable<long> GetChildPropertyIds(long propertyId) => m_propertyProperties[propertyId];
    }
}
