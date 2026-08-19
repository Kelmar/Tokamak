using System.Numerics;

namespace Tokamak.Tritium.Geometry
{
    public readonly record struct Bone
    {
        public required int Index { get; init; }

        public required int ParentIndex { get; init; }

        public required Matrix4x4 Transform { get; init; }

        public override string ToString() => $"B:{Index}";
    }
}
