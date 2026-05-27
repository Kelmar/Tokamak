namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal interface IFBXObject
    {
        long ID { get; }

        string Name { get; }

        Node Node { get; }
    }
}
