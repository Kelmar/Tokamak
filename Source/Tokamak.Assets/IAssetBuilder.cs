using System;

namespace Tokamak.Assets
{
    public interface IAssetBuilder
    {
        void NewModel(Action<ISceneObjectBuilder> configure);

        void NewMesh(Action<IMeshBuilder> configure);

        void NewMaterial(Action<IMaterialBuilder> configure);

        void BuildAll();
    }
}
