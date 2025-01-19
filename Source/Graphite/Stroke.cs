using System.Collections.Generic;
using System.Numerics;

using Tokamak.Mathematics;

namespace Graphite
{
    /// <summary>
    /// Represents a stroked shape to be drawn to the screen.
    /// </summary>
    internal class Stroke
    {
        public bool Closed { get; set; }

        public Pen Pen { get; set; }

        public List<StrokePoint> Points { get; } = new List<StrokePoint>();

        public void AddPoint(in Point p)
        {
            Points.Add(new StrokePoint
            {
                Position = new Vector2(p.X, p.Y),
                Flags = PointFlags.Corner
            });
        }

        public void MoveTo(in Point p)
        {
            AddPoint(p);
        }

        public void MoveTo(int x, int y) => MoveTo(new Point(x, y));

        public void LineTo(in Point p)
        {
            AddPoint(p);
        }

        public void LineTo(int x, int y) => LineTo(new Point(x, y));
    }
}
