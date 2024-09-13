using System;
using System.IO;
using System.Linq;
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
        //public const string FILE = "resources/cube_tris.fbx";
        //public const string FILE = "resources/cube.fbx";
        //public const string FILE = "resources/blox.fbx";
        public const string FILE = "resources/susan.fbx";
        //public const string FILE = "resources/plane.fbx";

        private readonly IVertexBuffer<VectorFormatPCT> m_vertexBuffer;
        private readonly IElementBuffer m_elementBuffer;

        private readonly Mesh m_mesh;

        private readonly IAPILayer m_apiLayer;

        public TestObject(IAPILayer apiLayer)
        {
            m_apiLayer = apiLayer;

            using var reader = new FBXReader(File.OpenRead(FILE));
            m_mesh = reader.Import().FirstOrDefault();

            if (m_mesh == null)
                throw new Exception($"Unable to load mesh.");

            m_vertexBuffer = m_apiLayer.GetVertexBuffer<VectorFormatPCT>(BufferUsage.Static);
            m_elementBuffer = m_apiLayer.GetElementBuffer(BufferUsage.Static);

            m_mesh.ToVertexBuffer(m_vertexBuffer);
            m_mesh.ToElementsBuffer(m_elementBuffer);
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
            m_elementBuffer.Dispose();
            m_vertexBuffer.Dispose();

            m_mesh.Dispose();

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
            m_elementBuffer.Activate();
            m_vertexBuffer.Activate();

            cmdList.DrawElements(m_mesh.Indicies.Count);
        }
    }
}
