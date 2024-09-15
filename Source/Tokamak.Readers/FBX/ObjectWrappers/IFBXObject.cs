namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal interface IFBXObject
    {
        int ID { get; }

        string Name { get; }

        Node Node { get; }
    }
}
