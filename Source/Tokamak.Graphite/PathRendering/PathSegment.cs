using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Tokamak.Graphite.PathRendering
{

    /// <summary>
    /// Computed line segment along our path.
    /// </summary>
    internal class PathSegment
    {
        public PathSegment(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;

            Direction = End - Start;

            float length = Direction.Length();

            if (length != 0) // Avoid NaN while normalizing
                Direction /= length;
        }

        public Vector2 Start { get; set; }

        public Vector2 End { get; set; }

        public Vector2 Direction { get; set; }
    }
}
