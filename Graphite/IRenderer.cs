using System;
using System.Collections.Generic;
using System.Numerics;

namespace Graphite
{
    public interface IRenderer : IDisposable
    {
        bool WireFrame { get; set; }

        bool Debug { get; set; }

        void SetViewport(int x, int y);

        void DebugPoint(in Vector2 v, in Color color);

        void DebugLine(in Vector2 v1, in Vector2 v2, in Color color);

        void DebugPath(Path path, in Color color);

        void Stroke(IEnumerable<Vector2> vectors, in Color color);

        void Flush();
    }
}
