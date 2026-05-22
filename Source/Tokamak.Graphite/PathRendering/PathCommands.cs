using System.Numerics;

namespace Tokamak.Graphite.PathRendering
{
    internal abstract class PathCommand { }

    internal class CloseCommand : PathCommand
    {
    }

    internal class MoveToCommand : PathCommand
    {
        public MoveToCommand(Vector2 point)
        {
            Point = point;
        }

        public MoveToCommand(float x, float y)
        {
            Point = new Vector2(x, y);
        }

        public Vector2 Point { get; set; }
    }

    internal class LineToCommand : PathCommand
    {
        public LineToCommand(Vector2 point)
        {
            Point = point;
        }

        public LineToCommand(float x, float y)
        {
            Point = new Vector2(x, y);
        }

        public Vector2 Point { get; set; }
    }

    internal class ArcToCommand(Vector2 center, Vector2 radius, float startAngle, float endAngle) : PathCommand
    {
        public Vector2 Center { get; set; } = center;

        public Vector2 Radius { get; set; } = radius;

        public float StartAngle { get; set; } = startAngle;

        public float EndAngle { get; set; } = endAngle;
    }

    internal class QuadToCommand(Vector2 control, Vector2 end) : PathCommand
    {
        public Vector2 Control { get; set; } = control;

        public Vector2 End { get; set; } = end;
    }

    internal class CubicToCommand(Vector2 control1, Vector2 control2, Vector2 end) : PathCommand
    {
        public Vector2 Control1 { get; set; } = control1;

        public Vector2 Control2 { get; set; } = control2;

        public Vector2 End { get; set; } = end;
    }

    internal class WindingCommand(Winding winding) : PathCommand
    {
        public Winding Winding { get; set; } = winding;
    }
}
