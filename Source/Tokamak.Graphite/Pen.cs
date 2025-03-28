using Tokamak.Mathematics;

namespace Tokamak.Graphite
{
    public struct Pen
    {
        public Pen()
        {
        }

        public LineCap LineCap { get; set; } = LineCap.ButtCap;

        public LineJoin LineJoin { get; set; } = LineJoin.Miter;

        public float Width { get; set; } = 1;

        public float MiterLimit { get; set; } = 20;

        public Color Color { get; set; } = Color.Black;

        /// <summary>
        /// Returns true if drawing with this pen would result in a no operation.
        /// </summary>
        public bool IsEmpty => (Width <= 0) || (Color.Alpha == 0);
    }

    public static class Pens
    {
        private static readonly Pen s_empty = new Pen
        {
            Width = 0,
            Color = Color.Black
        };

        private static readonly Pen s_black = new Pen
        {
            Color = Color.Black
        };

        public static ref readonly Pen Empty => ref s_empty;

        public static ref readonly Pen Black => ref s_black;
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
