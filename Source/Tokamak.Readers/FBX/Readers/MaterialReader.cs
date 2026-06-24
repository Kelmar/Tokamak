using System;
using System.Linq;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Mappers;

namespace Tokamak.Readers.FBX.Readers
{
    internal class MaterialReader : IFBXObjectReader
    {
        private readonly ReadState m_state;
        private readonly FBXObject m_fbxObject;

        public MaterialReader(ReadState state, FBXObject obj)
        {
            m_state = state;
            m_fbxObject = obj;
        }

        public void Process()
        {
            var result = m_fbxObject.MapTo<MaterialInfo>();

            result.Id = m_fbxObject.Id;
            result.Name = m_fbxObject.Name;

            string? shading = m_fbxObject.Node.Children
                .WithName("ShadingModel")
                .FirstOrDefault()
                ?.Properties[0].AsString();

            if (!String.IsNullOrWhiteSpace(shading))
                result.ShadingModel = shading.ToLower();

            m_state.Materials.Add(result);
        }
    }
}
