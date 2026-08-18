using System;
using System.Collections.Generic;
using System.Text;

namespace Tokamak.Import.Builders
{
    public record PolygonInfo
    {
        public required List<int> Vertices
        {
            get;
            init => field = value ?? [];
        }
    }
}
