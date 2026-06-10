using System;

using Tokamak.Assets;

namespace ReadersTests.Support
{
    internal sealed class RecordingMeshBuilder : IMeshBuilder
    {
        private readonly RecordingPolyBuilder m_polyBuilder;
        private readonly RecordingAssetBuilder m_assetBuilder;

        public RecordingMeshBuilder(RecordingAssetBuilder assetBuilder)
        {
            m_assetBuilder = assetBuilder;
            m_polyBuilder = new(this);
        }

        public IPolygonBuilder GetPolygonBuilder() => m_polyBuilder;

        public IMeshBuilder WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, "Name");

            m_assetBuilder.Meshes.Add(name);
            return this;
        }
    }
}
