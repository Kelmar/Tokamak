using System.Collections.Generic;
using System.Linq;

namespace Tokamak.Readers.FBX.Builders
{
    internal class ModelBuilder : FBXObject
    {
        public ModelBuilder(GlobalSettings settings, ObjectGraph objectGraph, Node node)
            : base(settings, objectGraph, node)
        {
            Materials = BuildMaterials().ToList();

            Meshes = BuildMeshes().ToList();
        }

        public IReadOnlyList<MaterialBuilder> Materials { get; }

        public IReadOnlyList<MeshBuilder> Meshes { get; }

        private IEnumerable<MaterialBuilder> BuildMaterials()
        {
            return ChildNodes
                .Where(n => n.Name.ToLower() == "material")
                .Select(node => new MaterialBuilder(this, node));
        }

        private IEnumerable<MeshBuilder> BuildMeshes()
        {
            return ChildNodes
                .Where(n => n.Name.ToLower() == "geometry")
                .Select(node => new MeshBuilder(this, node));
        }
    }
}
