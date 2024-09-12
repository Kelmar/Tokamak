using System.Numerics;

using Tokamak.Mathematics;

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
        public const string FILE = "resources/cube.fbx";

        private readonly IVertexBuffer<VectorFormatPCT> m_vertexBuffer;
        private readonly IElementBuffer m_elementBuffer;

        //private readonly Mesh m_mesh;

        private readonly IAPILayer m_apiLayer;

        private readonly int m_elementCnt;

        public TestObject(IAPILayer apiLayer)
        {
            m_apiLayer = apiLayer;

            /*
            using var reader = new FBXReader(File.OpenRead(FILE));
            m_mesh = reader.Import().FirstOrDefault();

            if (m_mesh == null)
                throw new Exception($"Unable to load mesh.");
            */

            var verts = new VectorFormatPCT[]
            {
                BuildVector(-.5f,-0.5f, 0),
                BuildVector(0.5f,-0.5f, 0),
                BuildVector(-.5f, 0.5f, 0),
                BuildVector(0.5f, 0.5f, 0)
                
            };

            var indices = new uint[]
            {
                0, 1, 2,
                3
            };

            m_elementCnt = indices.Length;

            m_vertexBuffer = m_apiLayer.GetVertexBuffer<VectorFormatPCT>(BufferUsage.Static);
            m_elementBuffer = m_apiLayer.GetElementBuffer(BufferUsage.Static);

            m_vertexBuffer.Set(verts);
            m_elementBuffer.Set(indices);

            //m_mesh.ToVertexBuffer(m_vertexBuffer);
            ///m_mesh.ToElementsBuffer(m_elementBuffer);
        }

        private VectorFormatPCT BuildVertex(float x, float y, float z)
        {
            return new VectorFormatPCT
            {
                Point = new Vector3(x, y, z),
                Color = (Vector4)Color.White,
                TexCoord = Vector2.Zero
            };
        }

        public override void Dispose()
        {
            //m_mesh.Dispose();

            m_elementBuffer.Dispose();
            m_vertexBuffer.Dispose();

            base.Dispose();
        }

        private VectorFormatPCT BuildVector(float x, float y, float z, Vector2 texCoord = default)
        {
            return new VectorFormatPCT
            {
                Point = new Vector3(x, y, z),
                Color = (Vector4)Color.White,
                TexCoord = texCoord
            };
        }

        public override void Render(ICommandList cmdList)
        {
            m_vertexBuffer.Activate();
            m_elementBuffer.Activate();

            cmdList.DrawElements(m_elementCnt);
        }
    }
}
