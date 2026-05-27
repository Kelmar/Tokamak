using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Readers.FBX
{
    /// <summary>
    /// Holds the mapping from the Connection/C nodes for the FBX object hierarchy.
    /// </summary>
    internal class ObjectGraph
    {
        private readonly ILookup<long, long> m_objectGraph;

        private readonly ILookup<long, long> m_objectProperties;

        private readonly ILookup<long, long> m_propertyObjects;

        private readonly ILookup<long, long> m_propertyProperties;

        public ObjectGraph(Node rootNode)
        {
            var connections = GetConnections(rootNode).ToList();

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
        }

        private IEnumerable<Connection> GetConnections(Node rootNode)
        {
            var connectionNode = rootNode.Children["Connections"].FirstOrDefault();

            if (connectionNode == null)
                yield break;

            foreach (var n in connectionNode.Children["C"])
            {
                Connection? c = Connection.Build(n);

                if (c != null)
                    yield return c;
            }
        }

        public IEnumerable<long> GetChildObjectIds(long objectId) => m_objectGraph[objectId];

        public IEnumerable<long> GetObjectPropertyIds(long objectId) => m_objectProperties[objectId];

        public IEnumerable<long> GetPropertyObjectIds(long propertyId) => m_propertyObjects[propertyId];

        public IEnumerable<long> GetChildPropertyIds(long propertyId) => m_propertyProperties[propertyId];
    }
}
