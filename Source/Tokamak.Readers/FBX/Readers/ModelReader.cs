using System;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Readers
{
    internal class ModelReader : IFBXObjectReader
    {
        private int m_importCount = 0;

        public ModelReader(ReadState state)
        {
            State = state;
        }

        public string ObjectType => "model";

        public ReadState State { get; }

        private string GetAssetName(FBXObject model)
        {
            if (String.IsNullOrEmpty(model.Name))
                return $"{State.FileName}_{m_importCount}";

            return model.Name;
        }

        private void ReadSceneObject(FBXObject obj)
        {
            if (obj.Parents.Any())
                return; // Only import root items.

            var materialIds = obj.Children
                .WithFBXType("Material")
                .Select(o => o.Id)
                .ToList();

            var meshIds = obj.Children
                .WithFBXType("Geometry")
                .Select(o => o.Id)
                .ToList();

            ++m_importCount;

            var sceneObject = new SceneObjectInfo
            {
                Id = obj.Id,
                Name = GetAssetName(obj),
                MaterialIds = materialIds,
                MeshIds = meshIds
            };

            State.SceneObjects.Add(sceneObject);
        }

        public void ReadObject(FBXObject obj)
        {
            // For now we only support reading "Mesh" models.
            if (obj.IsSubClass("Mesh"))
            {
                ReadSceneObject(obj);
                return;
            }
        }
    }
}
