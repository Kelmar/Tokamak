using System;
using System.Collections.Generic;

using Tokamak.Assets;

namespace ReadersTests.Support
{
    internal sealed class RecordSceneObjectBuilder : ISceneObjectBuilder
    {
        private RecordingAssetBuilder m_assetBuilder;

        public RecordSceneObjectBuilder(RecordingAssetBuilder owner)
        {
            m_assetBuilder = owner;
        }

        public ISceneObjectBuilder WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, "Name");

            m_assetBuilder.Models.Add(name);
            return this;
        }

        public ISceneObjectBuilder AddMeshes(params IEnumerable<string> names)
            => this;

        public ISceneObjectBuilder WithSkeleton(string name)
            => this;
    }
}
