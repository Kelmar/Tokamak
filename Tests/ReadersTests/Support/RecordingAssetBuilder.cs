using System;
using System.Collections.Generic;

using Tokamak.Assets;

namespace ReadersTests.Support
{
    /// <summary>
    /// Minimal <see cref="IAssetBuilder"/> that records the names handed to it so
    /// integration tests can assert what an import produced.
    /// </summary>
    internal sealed class RecordingAssetBuilder : IAssetBuilder
    {
        public List<string> Materials { get; } = [];

        public List<string> Meshes { get; } = [];

        public List<string> Models { get; } = [];

        public void NewMaterial(Action<IMaterialBuilder> configure)
            => configure(new MatBuilder(this));

        public void NewMesh(Action<IMeshBuilder> configure)
            => configure(new MeshB(this));

        public void NewModel(Action<ISceneObjectBuilder> configure)
            => configure(new ModelB(this));

        public void BuildAll() { }

        private sealed class MatBuilder(RecordingAssetBuilder owner) : IMaterialBuilder
        {
            public IMaterialBuilder WithName(string name)
            {
                owner.Materials.Add(name);
                return this;
            }
        }

        private sealed class MeshB(RecordingAssetBuilder owner) : IMeshBuilder
        {
            public IMeshBuilder WithName(string name)
            {
                owner.Meshes.Add(name);
                return this;
            }
        }

        private sealed class ModelB(RecordingAssetBuilder owner) : ISceneObjectBuilder
        {
            public ISceneObjectBuilder WithName(string name)
            {
                owner.Models.Add(name);
                return this;
            }

            public ISceneObjectBuilder AddMeshes(params IEnumerable<string> names) => this;
        }
    }
}
