using System;
using System.Collections.Generic;
using System.Text;

using Tokamak.Assets;

using Tokamak.Tritium.APIs;
using Tokamak.Utilities;

namespace Tokamak.Tritium.Geometry
{
    public class Skeleton : Asset
    {
        public Skeleton()
        {
        }

        public List<Bone> Bones
        {
            get;
            set => field = value ?? [];
        } = [];

        public void Update(float timeDelta)
        {
        }

        public void DebugDraw(ICommandList commandList)
        {

        }

        public override string ToString()
        {
            var sb = new StringBuilder(ID);

            if (Bones.Count > 0)
            {
                sb.Append("->");
                sb.Append(String.Join(":", Bones));
            }

            return sb.ToString().Truncate(32);
        }
    }
}
