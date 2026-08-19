using System;

namespace Tokamak.Import.Builders
{
    public interface IAssetBuilder
    {
        void NewSceneObject(string name, Action<ISceneObjectBuilder> configure);

        void NewSkeleton(SkeletonInfo skeleton);

        void NewMesh(MeshInfo mesh);

        void NewMaterial(Action<IMaterialBuilder> configure);
    }
}
