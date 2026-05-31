using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Tritium.Scene
{
    public class SceneMeshObject : SceneObject
    {
        private IVertexBuffer<VectorFormatPNCT> m_vertexBuffer;
        private IElementBuffer m_elementBuffer;

        private int m_indexCount;

        public SceneMeshObject(IGraphicsLayer graphicsLayer)
        {
            m_vertexBuffer = graphicsLayer.GetVertexBuffer<VectorFormatPNCT>(BufferUsage.Static);
            m_elementBuffer = graphicsLayer.GetElementBuffer(BufferUsage.Static);

            m_indexCount = 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_elementBuffer.Dispose();
                m_vertexBuffer.Dispose();
            }

            base.Dispose(disposing);
        }

        public void SetMesh(Mesh mesh)
        {
            m_indexCount = mesh.ToBuffer(m_vertexBuffer, m_elementBuffer);
        }

        public override void Render(ICommandList commandList)
        {
            if (m_indexCount == 0)
                return; // No data set, skip.

            m_elementBuffer.Activate();
            m_vertexBuffer.Activate();

            commandList.DrawElements(m_indexCount);
        }
    }
}
