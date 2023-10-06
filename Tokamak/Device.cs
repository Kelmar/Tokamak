using System;
using System.Numerics;

namespace Tokamak
{
    public abstract class Device : IDisposable
    {
        protected Device()
        {
        }

        public virtual void Dispose()
        {
        }

        bool WireFrame { get; set; }

        bool Debug { get; set; }

        public abstract void SetViewport(int x, int y);

        public abstract void DebugPoint(in Vector2 v, in Color color);

        public abstract void DebugLine(in Vector2 v1, in Vector2 v2, in Color color);

        //public abstract void DebugPath(Path path, in Color color);

        public abstract void Flush();
    }
}
