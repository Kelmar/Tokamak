using System;
using System.Collections.Generic;

using Tokamak.Import.Builders;

namespace ImportFBXTests.Support
{
    /// <summary>
    /// Minimal <see cref="IAssetBuilder"/> that records the names handed to it so
    /// integration tests can assert what an import produced.
    /// </summary>
    internal sealed partial class RecordingAssetBuilder : IAssetBuilder
    {
        public HashSet<string> Materials { get; } = [];

        public HashSet<string> Meshes { get; } = [];

        public HashSet<string> Skeletons { get; } = [];

        public HashSet<string> Models { get; } = [];

        public void NewMaterial(Action<IMaterialBuilder> configure)
            => configure(new RecordingMaterialBuilder(this));

        public void NewMesh(MeshInfo mesh)
        {
            //=> configure(new RecordingMeshBuilder(this));
            Meshes.Add(mesh.Name);
        }

        public void NewSkeleton(SkeletonInfo skeleton)
        {
            //=> configure(new RecordingSkeletonBuilder(this));
            Skeletons.Add(skeleton.Name);
        }

        public void NewSceneObject(string name, Action<ISceneObjectBuilder> configure)
        {
            Models.Add(name);
            configure(new RecordSceneObjectBuilder(this));
        }
        
        public void BuildAll() { }
    }
}
