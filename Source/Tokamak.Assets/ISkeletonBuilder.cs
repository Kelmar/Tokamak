using System;
using System.Collections.Generic;

namespace Tokamak.Assets
{
    public interface ISkeletonBuilder
    {
        ISkeletonBuilder WithName(string name);

        ISkeletonBuilder WithBones<T>(IEnumerable<T> bones, BoneConfigurator<T> config);
    }
}
