using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tokamak.Readers.FBX.Builders;
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
            return properties.MapTo<GlobalSettings>();
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
