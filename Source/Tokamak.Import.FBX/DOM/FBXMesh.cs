using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tokamak.Import.FBX.DOM;

internal class FBXMesh : ResultRecord
{
    [NotMapped]
    public long ModelId { get; set; }

    [NotMapped]
    public List<FBXVertex> Vertices
    {
        get;
        init => field = value ?? [];
    } = [];

    [NotMapped]
    public List<FBXPolygon> Polygons
    {
        get;
        init => field = value ?? [];
    } = [];
}