using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Graphite
{
    public class Point
    {
        public Vector2 Location { get; set; }

        public Vector2 Direction { get; set; }

        public Vector2 MiterDirection { get; set; }

        public float Length { get; set; }

        public PointFlags Flags { get; set; }
    }

    [Flags]
    public enum PointFlags
    {
        None       = 0x0000,
        Corner     = 0x0001,
        Left       = 0x0002,
        OuterBevel = 0x0004,
        InnerBevel = 0x0008
    }
}
