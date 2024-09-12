using System;
using System.Numerics;

namespace Tokamak.Tritium.APIs
{
    public interface ICommandList : IDisposable
    {
        Vector4 ClearColor { get; set; }

        IDisposable BeginScope();

        void ClearBuffers(GlobalBuffer buffers);

        void ClearBoundTexture();

        void DrawArrays(int vertexOffset, int vertexCount);

        void DrawElements(int length);
    }
}
