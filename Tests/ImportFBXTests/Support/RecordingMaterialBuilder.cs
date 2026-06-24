using System;

using Tokamak.Import.Builders;

namespace ImportFBXTests.Support
{
    internal sealed class RecordingMaterialBuilder : IMaterialBuilder
    {
        private readonly RecordingAssetBuilder m_assetBuilder;

        public RecordingMaterialBuilder(RecordingAssetBuilder assetBuilder)
        {
            m_assetBuilder = assetBuilder;
        }

        public IMaterialBuilder WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, "Name");

            m_assetBuilder.Materials.Add(name);
            return this;
        }
    }
}
