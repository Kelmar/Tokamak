using System;
using System.Collections.Generic;
using System.Text;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Readers.FBX.DOM
{
    internal class FBXMesh : FBXObject
    {
        public FBXMesh(ReadState state, Node node)
            : base(state, node)
        {
            Polygons = [];
        }

        public List<FBXPolygon> Polygons
        {
            get;
            set => field = value ?? [];
        }
    }
}
