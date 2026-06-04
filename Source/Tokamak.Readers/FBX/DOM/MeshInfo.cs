using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tokamak.Readers.FBX.DOM
{
    internal class MeshInfo : ResultRecord
    {
        [NotMapped]
        public long ModelId { get; set; }

        [NotMapped]
        public List<FBXPolygon> Polygons
        {
            get;
            set => field = value ?? [];
        } = [];
    }
}
