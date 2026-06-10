using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Assets
{
    public delegate void BoneConfigurator<T>(T bone, IBoneBuilder builder);

    public interface IBoneBuilder
    {
        IBoneBuilder WithName(string name);

        IBoneBuilder WithTransform(in Matrix4x4 transform);

        IBoneBuilder WithChildBones<T>(IEnumerable<T> children, BoneConfigurator<T> config);
    }
}
