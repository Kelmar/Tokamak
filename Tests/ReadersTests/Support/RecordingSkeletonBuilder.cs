using System;

using Tokamak.Assets;

namespace ReadersTests.Support
{
    internal class RecordingSkeletonBuilder : ISkeletonBuilder
    {
        private readonly RecordingAssetBuilder m_assetBuilder;

        public RecordingSkeletonBuilder(RecordingAssetBuilder assetBuilder)
        {
            m_assetBuilder = assetBuilder;
        }

        public ISkeletonBuilder WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, "Name");

            m_assetBuilder.Skeletons.Add(name);
            return this;
        }
    }
}
