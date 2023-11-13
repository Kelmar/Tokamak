using System;
using System.Numerics;

using Tokamak.Buffer;

namespace Tokamak
{
    public interface ICommandList : IDisposable
    {
        IPipeline Pipeline { get; set; }

        Vector4 ClearColor { get; set; }

        void Begin();

        void End();

        void ClearBuffers(GlobalBuffer buffers);

        void ClearBoundTexture();

        void DrawArrays(int vertexOffset, int vertexCount);

        void DrawElements(int length);
    }
}
