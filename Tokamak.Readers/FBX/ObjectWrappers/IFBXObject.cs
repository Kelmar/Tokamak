namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal interface IFBXObject
    {
        int ID { get; set; }

        string Name { get; set; }

        Node Node { get; set; }
    }
}
