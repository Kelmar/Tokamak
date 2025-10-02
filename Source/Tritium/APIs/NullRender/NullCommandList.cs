using System;
using System.Numerics;

using Tokamak.Utilities;

namespace Tokamak.Tritium.APIs.NullRender
{
    /// <summary>
    /// Dummy command list that does nothing.
    /// </summary>
    internal class NullCommandList : ICommandList
    {
        public void Dispose()
        {
        }

        public Vector4 ClearColor { get; set; }

        public IDisposable BeginScope() => Indisposable.Instance;

        public void ClearBoundTexture()
        {
        }

        public void ClearBuffers(GlobalBuffer buffers)
        {
        }

        public void DrawArrays(int vertexOffset, int vertexCount)
        {
        }

        public void DrawElements(int length)
        {
        }
    }
}
