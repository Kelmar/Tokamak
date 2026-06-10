using System;

namespace Tokamak.Assets
{
    public interface IAssetBuilder
    {
        void NewSceneObject(Action<ISceneObjectBuilder> configure);

        void NewSkeleton(Action<ISkeletonBuilder> configure);

        void NewMesh(Action<IMeshBuilder> configure);

        void NewMaterial(Action<IMaterialBuilder> configure);

        void BuildAll();
    }
}
