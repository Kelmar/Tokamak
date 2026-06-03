using System;
using System.Collections.Generic;
using System.Text;

using Tokamak.Assets;

namespace Tokamak.Tritium.Builders
{
    internal class MaterialBuilder : IMaterialBuilder
    {
        public string Name { get; private set; } = String.Empty;

        public IMaterialBuilder WithName(string name)
        {
            Name = name;
            return this;
        }
    }
}
