using System.Collections.Generic;
using System.IO;
using System.Linq;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Mappers;

namespace Tokamak.Readers.FBX
{
    internal class ReadState
    {
        public ReadState(string filePath, Node rootNode)
        {
            FilePath = filePath;
            FileName = Path.GetFileNameWithoutExtension(FilePath);

            RootNode = rootNode;
            Settings = ReadSettings();
            ObjectGraph = new FBXObjectGraph(this, RootNode);
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

        public string FilePath { get; }

        public string FileName { get; }

        public GlobalSettings Settings { get; }

        public Node RootNode { get; }

        public FBXObjectGraph ObjectGraph { get; }

        public List<MaterialInfo> Materials
        {
            get;
            set => field = value ?? [];
        } = [];

        public List<SceneObjectInfo> SceneObjects { get; } = [];

        public List<MeshInfo> Meshes
        {
            get;
            set => field = value ?? [];
        } = [];
    }
}
