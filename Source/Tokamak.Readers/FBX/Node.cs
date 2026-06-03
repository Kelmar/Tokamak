using System.Collections.Generic;

namespace Tokamak.Readers.FBX
{
    /// <summary>
    /// A data node inside of an FBX file.
    /// </summary>
    /// <remarks>
    /// FBX files are laid out in nodes, similar to how XML has a nested structure.
    /// 
    /// These nodes however do not correspond to the model/mesh/etc hierarchy that we
    /// need for recreating an object in our engine.
    /// 
    /// To do that we need to load the data nodes up and then look at the nodes under 
    /// the Connections data node.
    /// </remarks>
    internal class Node
    {
        public required string Name { get; init; }

        public required List<NodeProperty> Properties { get; init; }

        public required List<Node> Children { get; init; }

        public override string ToString() => $"{Name}: C:{Children.Count} P:{Properties.Count}";
    }
}
