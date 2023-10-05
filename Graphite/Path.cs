using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Graphite
{
    public class Path
    {
        public List<Point> Points { get; } = new List<Point>();

        internal bool Convex { get; set; }

        internal int BevelCount { get; set; } = 0;

        public bool Closed { get; set; }

        public void AddPoint(Vector2 location, PointFlags flags)
        {
            Points.Add(new Point
            {
                Location = location,
                Flags = flags
            });
        }

        public void MoveTo(Vector2 location) => AddPoint(location, PointFlags.Corner);

        public void MoveTo(float x, float y) => MoveTo(new Vector2(x, y));

        public void LineTo(Vector2 location) => AddPoint(location, PointFlags.Corner);

        public void LineTo(float x, float y) => LineTo(new Vector2(x, y));

        public void Rect(float x, float y, float width, float height)
        {
            MoveTo(x, y);
            LineTo(x, y + height);
            LineTo(x + width, y + height);
            LineTo(x + width, y);
            Closed = true;
        }
    }
}
