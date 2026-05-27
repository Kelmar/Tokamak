using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Readers.FBX.ObjectWrappers
{
    /// <summary>
    /// Class for building material from FBX node.
    /// </summary>
    internal class MaterialBuilder : IFBXObject
    {
        private const string DEFAULT_SHADING_MODEL = "Phong";

        public MaterialBuilder(Node node)
        {
            Node = node;
            ID = Node.Properties[0].AsInt();
            Name = Node.Properties[1].AsString();

            ShadingModel = GetShadingProperty()?.AsString() ?? DEFAULT_SHADING_MODEL;

            Properties = CompoundProperty.BuildAllFor(node).ToList();

            Parameters = FBXReader.MapCompoundTo<MaterialParameters>(Properties);
        }

        private Property? GetShadingProperty()
            => Node.Children["ShadingModel"].FirstOrDefault()?.Properties[0];

        public int ID { get; }

        public string Name { get; }

        public string ShadingModel { get; }

        public Node Node { get; }

        public List<CompoundProperty> Properties { get; }

        public MaterialParameters Parameters { get; }

        public override string ToString() => $"{Name}: {ShadingModel}";
    }
}
