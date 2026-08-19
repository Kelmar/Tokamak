using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tokamak.Assets;

using Tokamak.Utilities;

namespace Tokamak.Tritium.Geometry
{
    public class Skeleton : Asset
    {
        private readonly Dictionary<string, int> m_boneNameIndexMap;
        
        public Skeleton(Dictionary<string, int> boneNameIndexMap, IEnumerable<Bone> bones)
        {
            m_boneNameIndexMap = boneNameIndexMap;
            Bones = bones.ToArray();
        }

        /// <summary>
        /// Get a bone's index from its name.
        /// </summary>
        /// <param name="name">The name of the bone to get the index for.</param>
        /// <returns>-1 if not found, otherwise the index of the bone.</returns>
        /// <exception cref="ArgumentException">Throw if the name parameter is invalid.</exception>
        public int BoneIndexByName(string name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            
            if (!m_boneNameIndexMap.TryGetValue(name, out int index))
                return -1;

            return index;
        }

        public Bone[] Bones { get; }

        public override string ToString()
        {
            var sb = new StringBuilder(ID);

            sb.Append(": ");
            sb.Append(String.Join(", ", Bones));

            return sb.ToString().Truncate(32);
        }
    }
}
