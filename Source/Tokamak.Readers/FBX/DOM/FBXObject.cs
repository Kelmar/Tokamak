using System;
using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Readers.FBX.DOM
{
    internal class FBXObject
    {
        public FBXObject(ReadState state, Node node)
        {
            State = state;
            Node = node;

            Id = Node.Properties[0].AsLong();

            (Name, SubClass) = ParseName();

            ChildNodes = ObjectGraph
                .GetChildObjects(Id)
                .ToList();

            Properties = ObjectProperty
                .BuildAllFor(node)
                .ToList();

            ParentIds = ObjectGraph.GetParentIds(Id).ToList();
            ParentNodes = ObjectGraph.GetParentObjects(Id).ToList();
        }

        private (string, string) ParseName()
        {
            string name = Node.Properties[1].AsString();

            int idx = name.IndexOf("::");

            return (idx != -1) ?
                (name.Substring(0, idx), name.Substring(idx + 2)) :
                (name, String.Empty);
        }

        public ReadState State { get; }

        public GlobalSettings Settings => State.Settings;

        /// <summary>
        /// The object graph for finding child objects.
        /// </summary>
        public ObjectGraph ObjectGraph => State.ObjectGraph;

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
        /// The parsed subclass (if any)
        /// </summary>
        public string SubClass { get; }

        /// <summary>
        /// The FBX node we read from.
        /// </summary>
        public Node Node { get; }

        /// <summary>
        /// List of child nodes that are owned by this object.
        /// </summary>
        public IEnumerable<Node> ChildNodes { get; }

        /// <summary>
        /// List of parent node IDs that own this object.
        /// </summary>
        public List<long> ParentIds { get; }

        /// <summary>
        /// List of parent nodes that own this object.
        /// </summary>
        public List<Node> ParentNodes { get; }

        /// <summary>
        /// List of compound properties detected for this node.
        /// </summary>
        public List<ObjectProperty> Properties { get; }
    }
}
