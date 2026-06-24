using System.Collections.Generic;
using System.Numerics;

namespace Tokamak.Readers.FBX.DOM
{
    /// <summary>
    /// A single polygon inside of a MeshInfo.
    /// </summary>
    internal class FBXPolygon
    {
        public int Index { get; set; }

        public List<int> Vertices
        {
            get;
            set => field = value ?? [];
        } = [];
    }
}
