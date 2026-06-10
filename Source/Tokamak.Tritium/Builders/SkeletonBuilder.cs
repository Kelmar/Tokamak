using System;
using System.Collections.Generic;
using System.Numerics;

using Tokamak.Assets;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Builders
{
    internal class SkeletonBuilder : ISkeletonBuilder
    {
        private class BoneBuilder : IBoneBuilder
        {
            public string Name { get; private set; } = String.Empty;

            public Matrix4x4 Transform { get; private set; } = Matrix4x4.Identity;

            public List<Bone> Children { get; } = [];

            public IBoneBuilder WithName(string name)
            {
                Name = name;
                return this;
            }

            public IBoneBuilder WithTransform(in Matrix4x4 transform)
            {
                Transform = transform;
                return this;
            }

            public IBoneBuilder WithChildBones<T>(IEnumerable<T> children, BoneConfigurator<T> config)
            {
                foreach (var bone in children)
                {
                    var childBuilder = new BoneBuilder();
                    config(bone, childBuilder);
                    Children.Add(childBuilder.Build());
                }

                return this;
            }

            public Bone Build()
            {
                //Validate();

                return new Bone
                {
                    Name = Name,
                    Transform = Transform,
                    Children = Children,
                };
            }
        }

        private readonly AssetManager m_assetManager;

        public SkeletonBuilder(AssetManager assetManager)
        {
            m_assetManager = assetManager;
        }

        public string Name { get; private set; } = String.Empty;

        public List<Bone> Bones { get; } = [];

        public ISkeletonBuilder WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(Name));

            Name = name;
            return this;
        }

        public ISkeletonBuilder WithBones<T>(IEnumerable<T> bones, BoneConfigurator<T> config)
        {
            foreach (var b in bones)
            {
                var bb = new BoneBuilder();
                config(b, bb);
                Bones.Add(bb.Build());
            }

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
            Validate();

            var skeleton = new Skeleton
            {
                Bones = Bones
            };

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
