using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX
{
    internal interface IParser
    {
        Node? ReadNode();
    }
}
