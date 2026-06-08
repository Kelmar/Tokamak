using System;
using System.Collections.Generic;

using Tokamak.Assets;
using Tokamak.Tritium.APIs;

namespace Tokamak.Tritium.Builders
{
    public class TritiumAssetBuilder : IAssetBuilder
    {
        private readonly AssetManager m_assetManager;
        private readonly IGraphicsLayer m_gfxLayer;

        private readonly List<Action<IMaterialBuilder>> m_materialConfig = [];
        private readonly List<Action<IMeshBuilder>> m_meshConfig = [];
        private readonly List<Action<ISceneObjectBuilder>> m_objectConfig = [];

        public TritiumAssetBuilder(AssetManager assetManager, IGraphicsLayer gfxLayer)
        {
            m_assetManager = assetManager;
            m_gfxLayer = gfxLayer;
        }

        public void NewMaterial(Action<IMaterialBuilder> configure)
            => m_materialConfig.Add(configure);

        public void NewMesh(Action<IMeshBuilder> configure)
            => m_meshConfig.Add(configure);

        public void NewModel(Action<ISceneObjectBuilder> configure)
            => m_objectConfig.Add(configure);

        public void BuildAll()
        {
            foreach (var config in m_materialConfig)
            {
                var materialBuilder = new MaterialBuilder();
                config(materialBuilder);
                materialBuilder.Build();
            }

            foreach (var config in m_meshConfig)
            {
                var meshBuilder = new MeshBuilder(m_assetManager, m_gfxLayer);
                config(meshBuilder);
                meshBuilder.Build();
            }

            foreach (var config in m_objectConfig)
            {
                var objectBuilder = new SceneObjectBuilder(m_assetManager, m_gfxLayer);
                config(objectBuilder);
                objectBuilder.Build();
            }

            m_materialConfig.Clear();
            m_meshConfig.Clear();
            m_objectConfig.Clear();
        }
    }
}
