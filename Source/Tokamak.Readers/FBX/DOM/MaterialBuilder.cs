using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Readers.FBX.Builders
{
    /// <summary>
    /// Class for building material from FBX node.
    /// </summary>
    internal class MaterialBuilder : FBXObject
    {
        private const string DEFAULT_SHADING_MODEL = "Phong";

        public MaterialBuilder(ModelBuilder parent, Node node)
            : base(parent, node)
        {
            ShadingModel = GetShadingProperty()?.AsString() ?? DEFAULT_SHADING_MODEL;

            Parameters = FBXReader.MapCompoundTo<MaterialParameters>(Properties);
        }

        private Property? GetShadingProperty()
            => Node.Children["ShadingModel"].FirstOrDefault()?.Properties[0];

        public string ShadingModel { get; }

        public MaterialParameters Parameters { get; }

        public override string ToString() => $"{Name}: {ShadingModel}";
    }
}
