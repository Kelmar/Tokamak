namespace Tokamak.Readers.FBX.ObjectWrappers
{
    /// <summary>
    /// Class for building material from FBX node.
    /// </summary>
    internal class MaterialBuilder : IFBXObject
    {
        public MaterialBuilder(Node node)
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
