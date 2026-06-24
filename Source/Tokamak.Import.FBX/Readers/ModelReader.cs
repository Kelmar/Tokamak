using System;
using System.Linq;

using Tokamak.Import.FBX.DOM;

namespace Tokamak.Import.FBX.Readers
{
    internal class ModelReader : IFBXObjectReader
    {
        private readonly ReadState m_state;
        private readonly FBXObject m_fbxObject;

        public ModelReader(ReadState state, FBXObject obj)
        {
            m_state = state;
            m_fbxObject = obj;
        }

        private void ReadSceneObject()
        {
            if (m_fbxObject.Parents.Any())
                return; // Only import root items.

            var materialIds = m_fbxObject.Children
                .WithFBXType("Material")
                .Select(o => o.Id)
                .ToList();

            var meshIds = m_fbxObject.Children
                .WithFBXType("Geometry")
                .Select(o => o.Id)
                .ToList();

            var sceneObject = new SceneObjectInfo
            {
                Id = m_fbxObject.Id,
                Name = m_fbxObject.Name,
                MaterialIds = materialIds,
                MeshIds = meshIds
            };

            m_state.SceneObjects.Add(sceneObject);
        }

        public void Process()
        {
            // For now we only support reading "Mesh" models.
            if (m_fbxObject.IsSubClass("Mesh"))
            {
                ReadSceneObject();
                return;
            }
        }
    }
}
