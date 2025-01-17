using System.Collections.Generic;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Quill.Readers.TTF
{
    internal class TTFSimpleGlyph : ITTFGlyph
    {
        public class Point
        {
            public Vector2 Value { get; set; }

            /// <summary>
            /// Flag indicates this is a handle (non-control) point for a beizer curve.
            /// </summary>
            public bool OnCurve { get; set; }

            public override string ToString()
            {
                var flag = OnCurve ? 'H' : 'C';

                return $"{flag}:{Value}";
            }
        }

        public class Contour
        {
            public List<Point> Points { get; set; } = new();
        }

        public required int Index { get; init; }

        public RectF Bounds { get; set; }

        public List<Contour> Contours { get; set; } = new();

        public List<byte> Instructions { get; set; } = new();
    }
}
