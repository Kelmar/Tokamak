using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal class ModelBuilder : IFBXObject
    {
        public ModelBuilder(ObjectGraph objectGraph, Node node)
        {
            Node = node;

            ID = Node.Properties[0].AsInt();
            Name = Node.Properties[1].AsString();

            Properties = CompoundProperty
                .BuildAllFor(node)
                .ToList();
        }

        public int ID { get; }

        public string Name { get; }

        public Node Node { get; }

        public List<CompoundProperty> Properties { get; }
    }
}
