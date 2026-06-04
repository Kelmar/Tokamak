using System;
using System.Linq;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Readers
{
    internal class ModelReader
    {
        public ModelReader(ReadState state)
        {
            State = state;
        }

        public ReadState State { get; }

        public ModelInfo? ReadModel(FBXObject obj)
        {
            if (!StringComparer.OrdinalIgnoreCase.Equals(obj.SubClass, "Mesh"))
                return null; // For now we only support reading "Mesh" models.

            if (obj.Parents.Any())
                return null; // Only import root items.

            var materialIds = obj.Children
                .WithFBXType("Material")
                .Select(o => o.Id)
                .ToList();

            var meshIds = obj.Children
                .WithFBXType("Geometry")
                .Select(o => o.Id)
                .ToList();

            return new ModelInfo
            {
                Id = obj.Id,
                Name = obj.Name,
                MaterialIds = materialIds,
                MeshIds = meshIds
            };
        }
    }
}
