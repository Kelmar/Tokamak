using System;
using System.Collections.Generic;
using System.Numerics;

using Tokamak.Assets;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Builders
{
    internal class BoneBuilder : IBoneBuilder
    {
        private readonly SkeletonBuilder m_skeletonBuilder;
        private readonly int m_parentIndex;
        private readonly int m_index;

        private readonly List<Action<IBoneBuilder>> m_childConfigs = [];

        public BoneBuilder(SkeletonBuilder skeletonBuilder, int parentIndex = -1)
        {
            m_skeletonBuilder = skeletonBuilder;

            m_parentIndex = parentIndex;
            m_index = m_skeletonBuilder.GetNextBoneIndex();
        }

        public string Name { get; private set; } = String.Empty;

        public List<int> Indices { get; } = [];

        public List<float> Weights { get; } = [];

        public Matrix4x4 Transform { get; private set; } = Matrix4x4.Identity;

        public IBoneBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        public IBoneBuilder ForIndices(IEnumerable<int> indices)
        {
            Indices.AddRange(indices);
            return this;
        }

        public IBoneBuilder WithWeights(IEnumerable<float> weights)
        {
            Weights.AddRange(weights);
            return this;
        }

        public IBoneBuilder WithTransform(in Matrix4x4 transform)
        {
            Transform = transform;
            return this;
        }

        public IBoneBuilder AddChild(Action<IBoneBuilder> config)
        {
            m_childConfigs.Add(config);
            return this;
        }

        public IBoneBuilder WithChildBones<T>(IEnumerable<T> source, BoneConfigurator<T> config)
        {
            foreach (var item in source)
                AddChild(bb => config(item, bb));

            return this;
        }

        public void Build()
        {
            //Validate();

            foreach (var config in m_childConfigs)
            {
                var builder = new BoneBuilder(m_skeletonBuilder, m_index);
                config(builder);
                builder.Build();
            }

            var result = new Bone
            {
                Name = Name,
                Transform = Transform,
                Index = m_index,
                ParentIndex = m_parentIndex
            };

            result.Indices.AddRange(Indices);
            result.Weights.AddRange(Weights);

            m_skeletonBuilder.Bones.Add(result);
        }
    }
}
