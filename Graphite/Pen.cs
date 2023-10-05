using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphite
{
    public class Pen
    {
        public LineCap LineCap { get; set; } = LineCap.ButtCap;

        public LineJoin LineJoin { get; set; } = LineJoin.Miter;

        public float Width { get; set; } = 1;

        public float MiterLimit { get; set; } = 2;

        public Color Color { get; set; }
    }

    public enum LineCap
    {
        ButtCap = 0,
    }

    public enum LineJoin
    {
        Miter = 0,
        Bevel = 1
    }
}
