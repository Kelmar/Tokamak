using System;
using System.Collections.Generic;

namespace Tokamak.Readers.FBX
{
    /// <summary>
    /// A data node inside of an FBX file.
    /// </summary>
    /// <remarks>
    /// FBX files are layed out in nodes, similar to how XML has a nested structure.
    /// 
    /// These nodes however do not correspond to the model/mesh/etc hierarchy that we
    /// need for recreating an object in our engine.
    /// 
    /// To do that we need to load the data nodes up and then look at the nodes under 
    /// the Connections data node.
    /// </remarks>
    internal class Node
    {
        private readonly IDictionary<string, List<Node>> m_children =
            new Dictionary<string, List<Node>>(StringComparer.CurrentCultureIgnoreCase);

        private List<Property> m_properties = new List<Property>();

        public string Name { get; set; }

        public void AddChild(Node child)
        {
            if (!m_children.TryGetValue(child.Name, out List<Node> children))
            {
                children = new List<Node>();
                m_children[child.Name] = children;
            }

            children.Add(child);
        }

        public IEnumerable<Node> GetChildren(string name)
        {
            if (!m_children.TryGetValue(name, out List<Node> children))
                return new List<Node>();

            return children;
        }

        public List<Property> Properties => m_properties;
    }
}
