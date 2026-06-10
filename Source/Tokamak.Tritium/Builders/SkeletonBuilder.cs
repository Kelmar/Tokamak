using System;

using Tokamak.Assets;

namespace Tokamak.Tritium.Builders
{
    internal class SkeletonBuilder : ISkeletonBuilder
    {

        public string Name { get; private set; } = String.Empty;

        public ISkeletonBuilder WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(Name));

            Name = name;
            return this;
        }

        public void Build()
        {
        }
    }
}
