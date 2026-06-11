using System;
using System.Collections.Generic;

namespace Tokamak.Assets
{
    public interface ISkeletonBuilder
    {
        ISkeletonBuilder WithName(string name);

        ISkeletonBuilder AddBone(Action<IBoneBuilder> config);

        ISkeletonBuilder WithBones<T>(IEnumerable<T> source, BoneConfigurator<T> config);
    }
}
