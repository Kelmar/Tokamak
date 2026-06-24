using System;

namespace Tokamak.Import.Builders
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
