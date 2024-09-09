using System;
using System.Numerics;

namespace Tokamak.Tritium
{
    public interface ICommandList : IDisposable
    {
        Vector4 ClearColor { get; set; }

        void Begin();

        void End();

        void ClearBuffers(GlobalBuffer buffers);

        void ClearBoundTexture();

        void DrawArrays(int vertexOffset, int vertexCount);

        void DrawElements(int length);
    }
}
