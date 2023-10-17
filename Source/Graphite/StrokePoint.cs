using System.Numerics;

namespace Graphite
{
    internal class StrokePoint
    {
        public Vector2 Position { get; set; }

        public Vector2 Direction { get; set; }

        public Vector2 MiterDirection { get; set; }

        public float Length { get; set; }

        public PointFlags Flags { get; set; }

        public void ComputeDirectionTo(StrokePoint other)
        {
            Direction = other.Position - Position;
            Length = Direction.Length();
            Direction /= (Length > 0) ? Length : 1;
        }
    }
}
