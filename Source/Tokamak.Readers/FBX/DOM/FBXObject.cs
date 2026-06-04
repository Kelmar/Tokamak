using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Readers.FBX.Mappers;

namespace Tokamak.Readers.FBX.DOM
{
    /// <summary>
    /// Represents a generic object from the /Objects node in an FBX file.
    /// </summary>
    internal class FBXObject
    {
        public FBXObject(ObjectGraph objectGraph, Node node)
        {
            ObjectGraph = objectGraph;
            Node = node;

            Id = ParseId();
            (Name, Class) = ParseNameClass();
            SubClass = Node.StringProperty(2);

            Properties = ObjectProperty
                .BuildAllFor(node)
                .ToList();
        }

        private long ParseId()
            => Node.Properties.Count < 1 ? -1 : Node.Properties[0].AsLong();

        private (string, string) ParseNameClass()
        {
            string name = Node.StringProperty(1);

            int idx = name.IndexOf("::");

            return (idx != -1) ?
                (name.Substring(0, idx), name.Substring(idx + 2)) :
                (name, String.Empty);
        }

        /// <summary>
        /// The object graph for finding child objects.
        /// </summary>
        public ObjectGraph ObjectGraph { get; }

        /// <summary>
        /// The unique ID of this object in the file.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// The name of this mesh in the file.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The type of object this is.
        /// </summary>
        /// <remarks>
        /// This is the node's name.
        /// </remarks>
        public string Type => Node.Name;

        /// <summary>
        /// The parsed class (if any)
        /// </summary>
        public string Class { get; }

        /// <summary>
        /// The parsed subclass (if any)
        /// </summary>
        public string SubClass { get; }

        /// <summary>
        /// The FBX node we read from.
        /// </summary>
        public Node Node { get; }

        /// <summary>
        /// List of child FBXObjects that are owned by this object.
        /// </summary>
        public IEnumerable<FBXObject> Children => ObjectGraph.GetChildObjects(Id);

        /// <summary>
        /// List of child nodes that are owned by this object.
        /// </summary>
        public IEnumerable<Node> ChildNodes => ObjectGraph.GetChildNodes(Id).ToList();

        /// <summary>
        /// List of parent node IDs that own this object.
        /// </summary>
        public List<long> ParentIds => ObjectGraph.GetParentIds(Id).ToList();

        /// <summary>
        /// List of parent nodes that own this object.
        /// </summary>
        public List<Node> ParentNodes => ObjectGraph.GetParentNodes(Id).ToList();

        /// <summary>
        /// List of parent objects that own this object.
        /// </summary>
        public IEnumerable<FBXObject> Parents => ObjectGraph.GetParentObjects(Id);

        /// <summary>
        /// List of compound properties detected for this node.
        /// </summary>
        public List<ObjectProperty> Properties { get; }

        public override string ToString() => $"{Type}: {Id} {Name}";
    }
}
