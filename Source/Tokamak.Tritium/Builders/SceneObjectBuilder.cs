using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Assets;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Geometry;
using Tokamak.Tritium.Scene;

namespace Tokamak.Tritium.Builders
{
    public class SceneObjectBuilder : ISceneObjectBuilder
    {
        private readonly AssetManager m_assetManager;

        private readonly IGraphicsLayer m_gfxLayer;

        public SceneObjectBuilder(AssetManager assetManager, IGraphicsLayer gfxLayer)
        {
            m_assetManager = assetManager;
            m_gfxLayer = gfxLayer;
        }

        public string Name { get; private set; } = String.Empty;

        public string SkeletonName { get; private set; } = String.Empty;

        public List<string> Meshes { get; private set; } = [];

        public ISceneObjectBuilder WithName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(Name));

            Name = name;
            return this;
        }

        public ISceneObjectBuilder AddMeshes(params IEnumerable<string> names)
        {
            Meshes = names.Where(n => !String.IsNullOrWhiteSpace(n)).ToList();
            return this;
        }

        public ISceneObjectBuilder WithSkeleton(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(SkeletonName));

            SkeletonName = name;
            return this;
        }

        private void Validate()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(Name, nameof(Name));

            if (Meshes.Count == 0)
                throw new ArgumentException(nameof(Meshes));
        }

        public void Build()
        {
            Validate();

            var obj = new SceneMeshObject();

            try
            {
                int loadCnt = 0;

                foreach (var meshName in Meshes)
                {
                    var mesh = m_assetManager.Find<Mesh>(meshName);

                    if (mesh == null)
                        continue;
                    
                    obj.AddMesh(mesh);
                    ++loadCnt;
                }

                if (loadCnt == 0)
                    throw new Exception("Unable to load any of the named meshes.");

                m_assetManager.RegisterAsset(Name, obj);
            }
            catch
            {
                obj.Dispose();
                throw;
            }
        }
    }
}
