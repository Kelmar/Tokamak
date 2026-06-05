using Tokamak.Assets;

namespace ReadersTests.Support
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
            m_assetBuilder.Materials.Add(name);
            return this;
        }
    }
}
