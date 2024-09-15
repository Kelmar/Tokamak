using System;
using System.IO;
using System.Linq;

using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;

using Tokamak.Readers.FBX;

using TestBed.Scenes;
using Tokamak.Tritium.APIs;

//using Tokamak.Scenes;

namespace TestBed
{
    public class TestObject : SceneObject
    {
        //public const string FILE = "resources/cube.fbx";
        //public const string FILE = "resources/blox.fbx";
        public const string FILE = "resources/susan.fbx";
        //public const string FILE = "resources/plane.fbx";

        private readonly IVertexBuffer<VectorFormatPNCT> m_vertexBuffer;
        private readonly IElementBuffer m_elementBuffer;

        private readonly Mesh m_mesh;

        private readonly IAPILayer m_apiLayer;

        private readonly int m_indexCount;

        public TestObject(IAPILayer apiLayer)
        {
            m_apiLayer = apiLayer;

            using var reader = new FBXReader(File.OpenRead(FILE));
            m_mesh = reader.Import().FirstOrDefault();

            if (m_mesh == null)
                throw new Exception($"Unable to load mesh.");

            m_vertexBuffer = m_apiLayer.GetVertexBuffer<VectorFormatPNCT>(BufferUsage.Static);
            m_elementBuffer = m_apiLayer.GetElementBuffer(BufferUsage.Static);

            m_indexCount = m_mesh.ToBuffer(m_vertexBuffer, m_elementBuffer);
        }

        public override void Dispose()
        {
            m_elementBuffer.Dispose();
            m_vertexBuffer.Dispose();

            base.Dispose();
        }

        public override void Render(ICommandList cmdList)
        {
            m_elementBuffer.Activate();
            m_vertexBuffer.Activate();

            cmdList.DrawElements(m_indexCount);
        }
    }
}
