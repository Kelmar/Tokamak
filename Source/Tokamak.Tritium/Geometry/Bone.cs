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

        public Matrix4x4 Transform { get; set; }

        public List<Bone> Children
        {
            get;
            set => field = value ?? [];
        } = [];

        public override string ToString()
        {
            var sb = new StringBuilder(Name);

            if (Children.Count > 0)
            {
                sb.Append("->");
                sb.Append(String.Join(":", Children));
            }

            return sb.ToString().Truncate(32);
        }
    }
}
