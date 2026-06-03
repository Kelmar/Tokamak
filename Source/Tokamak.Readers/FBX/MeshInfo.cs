using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tokamak.Readers.FBX
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
