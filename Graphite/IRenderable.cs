using System;

using Tokamak.Mathematics;

namespace Graphite
{
    public interface IRenderable : IDisposable
    {
        void Resize(in Point size);

        void Render();
    }
}
