using System.Collections.Generic;
using System.Linq;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Mappers
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

            Objects = GetObjects();
        }

        public Dictionary<long, FBXObject> Objects { get; }

        private IEnumerable<Connection> GetConnections()
        {
            var connectionNode = m_rootNode.Children
                .WithName("Connections")
                .FirstOrDefault();

            if (connectionNode == null)
                yield break;

            foreach (var n in connectionNode.Children.WithName("C"))
            {
                Connection? c = Connection.Build(n);

                if (c != null)
                    yield return c;
            }
        }

        /// <summary>
        /// Get all nodes as a dictionary where they can be looked up by ID.
        /// </summary>
        /// <returns></returns>
        private Dictionary<long, FBXObject> GetObjects()
        {
            var objectNodes = m_rootNode.Children
                .WithName("Objects")
                .SelectMany(o => o.Children)
                .ToList();

            /*
             * Thowing out duplicate IDs for now, but do we need to account for object types as well?
             * 
             * Something to review if the FBX format allows for this or not.
             */

            return objectNodes
                .Select(n => new FBXObject(this, n))
                .DistinctBy(o => o.Id)
                .ToDictionary(x => x.Id, x => x);
        }

        public IEnumerable<FBXObject> GetObjectsOfType(string type)
            => Objects.Values.WithFBXType(type);

        public IEnumerable<long> GetParentIds(long childId) => m_parentGraph[childId].Where(id => id != 0);

        public IEnumerable<Node> GetParentNodes(long childId)
        {
            foreach (var id in GetParentIds(childId))
            {
                if (Objects.TryGetValue(id, out var obj))
                    yield return obj.Node;
            }
        }

        public IEnumerable<FBXObject> GetParentObjects(long childId)
        {
            var parentIds = GetParentIds(childId);

            foreach (long id in parentIds)
            {
                if (Objects.TryGetValue(id, out var obj))
                    yield return obj;
            }    
        }

        public IEnumerable<long> GetChildObjectIds(long parentId) => m_objectGraph[parentId];

        public IEnumerable<FBXObject> GetChildObjects(long parentId)
        {
            var childIds = GetChildObjectIds(parentId);

            foreach (long id in childIds)
            {
                if (Objects.TryGetValue(id, out var obj))
                    yield return obj;
            }
        }

        public IEnumerable<Node> GetChildNodes(long parentId)
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
