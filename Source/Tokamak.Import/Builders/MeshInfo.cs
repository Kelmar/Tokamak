using System.Collections.Generic;

namespace Tokamak.Import.Builders
{
    public record MeshInfo
    {
        public required string Name { get; set; }

        public required List<VertexInfo> Vertices
        {
            get;
            set => field = value ?? [];
        }

        public required List<PolygonInfo> Polygons
        {
            get;
            init => field = value ?? [];
        }
    }
}
