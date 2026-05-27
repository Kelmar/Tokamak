using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal class ModelBuilder : IFBXObject
    {
        public ModelBuilder(GlobalSettings settings, ObjectGraph objectGraph, Node node)
        {
            Settings = settings;

            Node = node;

            ID = Node.Properties[0].AsLong();
            Name = Node.Properties[1].AsString();

            Properties = CompoundProperty
                .BuildAllFor(node)
                .ToList();

            Children = objectGraph.GetChildObjects(ID).ToList();

            Materials = BuildMaterials().ToList();

            Meshes = BuildMeshes().ToList();
        }

        public GlobalSettings Settings { get; }

        public long ID { get; }

        public string Name { get; }

        public Node Node { get; }

        public List<CompoundProperty> Properties { get; }

        public List<Node> Children { get; }

        public IReadOnlyList<MaterialBuilder> Materials { get; }

        public IReadOnlyList<MeshBuilder> Meshes { get; }

        private IEnumerable<MaterialBuilder> BuildMaterials()
        {
            return Children
                .Where(n => n.Name.ToLower() == "material")
                .Select(node => new MaterialBuilder(node));
        }

        private IEnumerable<MeshBuilder> BuildMeshes()
        {
            return Children
                .Where(n => n.Name.ToLower() == "geometry")
                .Select(node => new MeshBuilder(Settings, Materials.ToArray(), node));
        }
    }
}
