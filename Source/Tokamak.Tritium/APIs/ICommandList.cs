using System;

namespace Tokamak.Tritium.APIs
{
    public interface ICommandList : IDeviceResource
    {
        IDisposable BeginScope();

        void ClearBuffers(GlobalBuffer buffers);

        void ClearBoundTexture();

        void DrawArrays(int vertexOffset, int vertexCount);

        void DrawElements(int length);
    }
}
