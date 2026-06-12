using System;
using System.Buffers;
using System.Linq;
using System.Numerics;
using System.Text;

using Tokamak.Assets;

using Tokamak.Mathematics;

using Tokamak.Utilities;

namespace Tokamak.Tritium.Geometry
{
    public class Skeleton : Asset
    {
        private IMemoryOwner<Matrix4x4> m_matrices;

        public Skeleton(Bone[] bones)
        {
            // Duplicate the bone array so it can be reused by caller if they want.
            Bones = bones.ToArray();

            // Build flat list of bone matrices for access by shaders.
            m_matrices = MemoryPool<Matrix4x4>.Shared.Rent(Bones.Length);

            var mem = m_matrices.Memory.Span;

            for (int i = 0; i < Bones.Length; ++i)
                mem[i] = Bones[i].Transform;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                m_matrices.Dispose();

            base.Dispose(disposing);
        }

        public Bone[] Bones { get; }

        public ReadOnlyMemory<Matrix4x4> Matrices => m_matrices.Memory;

        public override string ToString()
        {
            var sb = new StringBuilder(ID);

            sb.Append(": ");
            sb.Append(String.Join(", ", Bones));

            return sb.ToString().Truncate(32);
        }
    }
}
