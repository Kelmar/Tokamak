using System;
using System.Linq;
using System.Text;

using Tokamak.Assets;

using Tokamak.Tritium.APIs;
using Tokamak.Utilities;

namespace Tokamak.Tritium.Geometry
{
    public class Skeleton : Asset
    {
        public Skeleton(Bone[] bones)
        {
            Bones = bones.ToArray(); // Duplicate the array.
        }

        public Bone[] Bones { get; }

        public void DebugDraw(ICommandList commandList)
        {

        }

        public override string ToString()
        {
            var sb = new StringBuilder(ID);

            sb.Append(": ");
            sb.Append(String.Join(", ", Bones));

            return sb.ToString().Truncate(32);
        }
    }
}
