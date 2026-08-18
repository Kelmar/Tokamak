using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Import.FBX.DOM
{
    /// <summary>
    /// A single polygon inside of a MeshInfo.
    /// </summary>
    internal record FBXPolygon
    {
        public int Index { get; set; }

        public List<int> Vertices
        {
            get;
            set => field = value ?? [];
        } = [];
    }
}
