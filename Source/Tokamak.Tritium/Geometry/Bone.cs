using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using Tokamak.Utilities;

namespace Tokamak.Tritium.Geometry
{
    public class Bone
    {
        public required string Name { get; init; }

        public int Index { get; set; }

        public int ParentIndex { get; set; }

        public List<int> Indices { get; } = [];

        public List<float> Weights { get; } = [];

        public Matrix4x4 Transform { get; set; }

        public override string ToString()
        {
            if (!String.IsNullOrWhiteSpace(Name))
                return Name;

            return Index.ToString();
        }
    }
}
