using System;

using Tokamak.Assets;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Builders
{
    internal class SkeletonBuilder : ISkeletonBuilder
    {
        private readonly AssetManager m_assetManager;

        public SkeletonBuilder(AssetManager assetManager)
        {
            m_assetManager = assetManager;
        }

        public string Name { get; private set; } = String.Empty;

        public ISkeletonBuilder WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(Name));

            Name = name;
            return this;
        }

        private void Validate()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Name, nameof(Name));
        }

        public void Build()
        {
            Validate();

            var skeleton = new Skeleton();

            try
            {
                m_assetManager.RegisterAsset(Name, skeleton);
            }
            catch
            {
                skeleton.Dispose();
                throw;
            }
        }
    }
}
