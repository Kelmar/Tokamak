using System.Collections.Generic;

namespace Tokamak.Readers.FBX.Builders
{
    internal interface IFBXObject
    {
        GlobalSettings Settings { get; }

        ObjectGraph ObjectGraph { get; }

        long ID { get; }

        string Name { get; }

        Node Node { get; }

        IEnumerable<Node> ChildNodes { get; }
    }
}
