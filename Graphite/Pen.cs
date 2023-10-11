using Tokamak;

namespace Graphite
{
    public class Pen
    {
        public LineCap LineCap { get; set; } = LineCap.ButtCap;

        public LineJoin LineJoin { get; set; } = LineJoin.Miter;

        public float Width { get; set; } = 1;

        public float MiterLimit { get; set; } = 20;

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
