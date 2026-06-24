using System;

using Tokamak.Import.Builders;

namespace Tokamak.Tritium.Builders
{
    internal class MaterialBuilder : IMaterialBuilder
    {
        public string Name { get; private set; } = String.Empty;

        public IMaterialBuilder WithName(string name)
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
