using System.Collections.Generic;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Scene
{
    public class SceneMeshObject : SceneObject
    {
        private readonly List<Mesh> m_meshes = [];

        public SceneMeshObject()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var mesh in m_meshes)
                    mesh.Dispose();
            }

            base.Dispose(disposing);
        }

        public void AddMesh(Mesh mesh)
        {
            m_meshes.Add(mesh);
        }

        public override void Render(ICommandList commandList)
        {
            foreach (var mesh in m_meshes)
            {
                if (!mesh.IsEmpty)
                    mesh.Draw(commandList);
            }
        }
    }
}
