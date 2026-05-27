using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Readers.FBX.Builders
{
    internal abstract class FBXObject : IFBXObject
    {
        private FBXObject(IFBXObject? parent, GlobalSettings settings, ObjectGraph objectGraph, Node node)
        {
            Parent = parent;
            Settings = settings;
            ObjectGraph = objectGraph;
            Node = node;

            ID = Node.Properties[0].AsLong();
            Name = Node.Properties[1].AsString();

            ChildNodes = ObjectGraph
                .GetChildObjects(ID)
                .ToList();
        }

        protected FBXObject(GlobalSettings settings, ObjectGraph objectGraph, Node node)
            : this(null, settings, objectGraph, node)
        {
        }

        protected FBXObject(IFBXObject parent, Node node)
            : this(parent, parent.Settings, parent.ObjectGraph, node)
        {
        }

        public GlobalSettings Settings { get; }

        /// <summary>
        /// The object graph for finding child objects.
        /// </summary>
        public ObjectGraph ObjectGraph { get; }

        /// <summary>
        /// The object which owns this child.
        /// </summary>
        /// <remarks>
        /// This value is NULL for root objects.
        /// </remarks>
        public IFBXObject? Parent { get; }

        /// <summary>
        /// The unique ID of this object in the file.
        /// </summary>
        public long ID { get; }

        /// <summary>
        /// The name of this mesh in the file.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The FBX node we read from.
        /// </summary>
        public Node Node { get; }

        /// <summary>
        /// List of child nodes that are owned by this object.
        /// </summary>
        public IEnumerable<Node> ChildNodes { get; }
    }
}
