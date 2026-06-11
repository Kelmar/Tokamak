using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Assets;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Builders
{
    internal class SkeletonBuilder : ISkeletonBuilder
    {
        private readonly AssetManager m_assetManager;

        private readonly List<Action<IBoneBuilder>> m_boneConfig = [];

        private int m_boneIndex = -1;

        public SkeletonBuilder(AssetManager assetManager)
        {
            m_assetManager = assetManager;
        }

        public string Name { get; private set; } = String.Empty;

        public List<Bone> Bones { get; } = [];

        internal int GetNextBoneIndex() => ++m_boneIndex;

        public ISkeletonBuilder WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(Name));

            Name = name;
            return this;
        }

        public ISkeletonBuilder AddBone(Action<IBoneBuilder> config)
        {
            m_boneConfig.Add(config);
            return this;
        }

        public ISkeletonBuilder WithBones<T>(IEnumerable<T> source, BoneConfigurator<T> config)
        {
            foreach (var item in source)
                AddBone(bb => config(item, bb));

            return this;
        }

        private void Validate()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Name, nameof(Name));

            if (Bones.Count == 0)
                throw new Exception($"Skeleton '{Name}' has no bones");
        }

        public void Build()
        {
            foreach (var config in m_boneConfig)
            {
                var builder = new BoneBuilder(this, -1);
                config(builder);
                builder.Build();
            }

            Validate();

            var skeleton = new Skeleton(Bones.OrderBy(b => b.Index).ToArray());

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
