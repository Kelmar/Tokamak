namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal class MaterialWrapper : IFBXObject
    {
        public MaterialWrapper(Node node)
        {
            Node = node;
            ID = Node.Properties[0].AsInt();
            Name = Node.Properties[1].AsString();
        }

        public int ID { get; }

        public string Name { get; }

        public Node Node { get; }
    }
}
