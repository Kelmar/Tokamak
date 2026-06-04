using System.Collections.Generic;
using System.Linq;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Mappers;

namespace Tokamak.Readers.FBX
{
    internal class ReadState
    {
        public ReadState(Node rootNode)
        {
            RootNode = rootNode;
            Settings = ReadSettings();
            ObjectGraph = new ObjectGraph(this, RootNode);
        }

        private GlobalSettings ReadSettings()
        {
            var node = RootNode.Children
                .WithName("GlobalSettings")
                .FirstOrDefault();

            if (node == null)
                return new GlobalSettings(); // Use default settings.

            var properties = ObjectProperty.BuildAllFor(node);
            var result = properties.MapTo<GlobalSettings>();

            result.Validate();

            return result;
        }

        public GlobalSettings Settings { get; }

        public Node RootNode { get; }

        public ObjectGraph ObjectGraph { get; }

        public List<MaterialInfo> Materials
        {
            get;
            set => field = value ?? [];
        } = [];

        public List<ModelInfo> Models
        {
            get;
            set => field = value ?? [];
        } = [];

        public List<MeshInfo> Meshes
        {
            get;
            set => field = value ?? [];
        } = [];
    }
}
