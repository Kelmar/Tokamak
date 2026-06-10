using System;
using System.Collections.Generic;

using Tokamak.Assets;

namespace ReadersTests.Support
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

        public void NewMesh(Action<IMeshBuilder> configure)
            => configure(new RecordingMeshBuilder(this));

        public void NewSkeleton(Action<ISkeletonBuilder> configure)
            => configure(new RecordingSkeletonBuilder(this));

        public void NewSceneObject(Action<ISceneObjectBuilder> configure)
            => configure(new RecordSceneObjectBuilder(this));

        public void BuildAll() { }
    }
}
