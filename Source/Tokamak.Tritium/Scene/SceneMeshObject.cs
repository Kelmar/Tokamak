using System.Collections.Generic;

using Tokamak.Assets;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Scene
{
    public class SceneMeshObject : SceneObject
    {
        private readonly List<AssetReference<Mesh>> m_meshes = [];

        private AssetReference<Skeleton>? m_skeleton = null;

        public SceneMeshObject()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_skeleton?.Dispose();

                foreach (var mesh in m_meshes)
                    mesh.Dispose();
            }

            base.Dispose(disposing);
        }

        public void AddMesh(AssetReference<Mesh> mesh)
        {
            m_meshes.Add(mesh);
        }

        public void SetSkeleton(AssetReference<Skeleton> skeleton)
        {
            m_skeleton = skeleton;
        }

        public override void Update(float timeDelta)
        {
            if (m_skeleton == null)
                return;

            m_skeleton.Asset.Update(timeDelta);
        }

        public override void Render(ICommandList commandList)
        {
            foreach (var mesh in m_meshes)
            {
                if (!mesh.Asset.IsEmpty)
                    mesh.Asset.Draw(commandList);
            }
        }
    }
}
