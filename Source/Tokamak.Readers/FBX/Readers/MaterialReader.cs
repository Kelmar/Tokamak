using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Mappers;

namespace Tokamak.Readers.FBX.Readers
{
    internal class MaterialReader : IFBXObjectReader
    {
        public MaterialReader(ReadState state)
        {
            State = state;
        }

        public string ObjectType => "material";

        public ReadState State { get; }

        public void ReadObject(FBXObject obj)
        {
            var result = obj.MapTo<MaterialInfo>();

            result.Id = obj.Id;
            result.Name = obj.Name;

            string? shading = obj.Node.Children
                .WithName("ShadingModel")
                .FirstOrDefault()
                ?.Properties[0].AsString();

            if (!String.IsNullOrWhiteSpace(shading))
                result.ShadingModel = shading.ToLower();

            State.Materials.Add(result);
        }
    }
}
