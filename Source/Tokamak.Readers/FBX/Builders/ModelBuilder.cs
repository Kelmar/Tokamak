using System.Collections.Generic;
using System.Linq;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Builders
{
    internal class ModelBuilder : Builder
    {
        public ModelBuilder(ReadState state)
            : base(state)
        {
            // Build the root models
            Models = ObjectGraph.GetChildObjects(0)
                .WithFBXType("model")
                .Select(BuildModel)
                .ToList();
        }

        public List<FBXModel> Models { get; }

        private FBXModel BuildModel(Node node)
        {
            var result = new FBXModel(State, node);

            result.MaterialIds = node.Children["materials"]
               .Select(n => n.Properties[0].AsLong())
               .ToList();

            result.MeshIds = node.Children["geometry"]
                .Select(n => n.Properties[0].AsLong())
                .ToList();

            return result;
        }
    }
}
