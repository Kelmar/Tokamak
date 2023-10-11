using Tokamak.Buffer;

namespace Graphite
{
    internal class CanvasCall
    {
        public int VertexOffset { get; set; }

        public int VertexCount { get; set; }

        public ITextureObject Texture { get; set; }
    }
}
