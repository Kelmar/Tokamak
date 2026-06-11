using System;
using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Assets
{
    public delegate void BoneConfigurator<T>(T bone, IBoneBuilder builder);

    public interface IBoneBuilder
    {
        IBoneBuilder WithName(string name);

        IBoneBuilder ForIndices(IEnumerable<int> indices);

        IBoneBuilder WithWeights(IEnumerable<float> weights);

        IBoneBuilder WithTransform(in Matrix4x4 transform);

        IBoneBuilder AddChild(Action<IBoneBuilder> config);

        IBoneBuilder WithChildBones<T>(IEnumerable<T> children, BoneConfigurator<T> config);
    }
}
