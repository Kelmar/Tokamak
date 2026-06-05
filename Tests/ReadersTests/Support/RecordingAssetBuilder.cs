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
        public List<string> Materials { get; } = [];

        public List<string> Meshes { get; } = [];

        public List<string> Models { get; } = [];

        public void NewMaterial(Action<IMaterialBuilder> configure)
            => configure(new RecordingMaterialBuilder(this));

        public void NewMesh(Action<IMeshBuilder> configure)
            => configure(new RecordingMeshBuilder(this));

        public void NewModel(Action<ISceneObjectBuilder> configure)
            => configure(new RecordingModelBuilder(this));

        public void BuildAll() { }
    }
}
