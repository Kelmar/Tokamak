using System;

using Tokamak.Mathematics;

namespace Tokamak
{
    public interface IRenderable : IDisposable
    {
        void Resize(in Point size);

        void Render();
    }
}
