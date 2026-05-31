using System;
using System.Collections.Generic;
using System.Text;

using Tokamak.Assets;

namespace Tokamak.Tritium.Geometry
{
    internal class MeshFactory : IAssetFactory
    {
        public MeshFactory()
        {

        }

        public Type ForType => typeof(Mesh);

        public Asset Build(string path)
        {
            return new Mesh();
        }
    }
}
