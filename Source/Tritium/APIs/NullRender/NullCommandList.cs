using System.Numerics;

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

        public void Begin()
        {
        }

        public void End()
        {
        }

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
