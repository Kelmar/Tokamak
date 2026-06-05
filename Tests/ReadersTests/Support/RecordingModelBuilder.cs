using System.Collections.Generic;

using Tokamak.Assets;

namespace ReadersTests.Support
{
    internal sealed class RecordingModelBuilder : ISceneObjectBuilder
    {
        private RecordingAssetBuilder m_assetBuilder;

        public RecordingModelBuilder(RecordingAssetBuilder owner)
        {
            m_assetBuilder = owner;
        }

        public ISceneObjectBuilder WithName(string name)
        {
            m_assetBuilder.Models.Add(name);
            return this;
        }

        public ISceneObjectBuilder AddMeshes(params IEnumerable<string> names)
            => this;
    }
}
