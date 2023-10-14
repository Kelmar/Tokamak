using System;
using System.IO;
using System.Linq;
using System.Numerics;

using Tokamak;
using Tokamak.Buffer;
using Tokamak.Formats;
using Tokamak.Readers.FBX;
using Tokamak.Scenes;

namespace TestBed
{
    public class TestObject : SceneObject
    {
        public const string FILE = "cube.fbx";

        private readonly IVertexBuffer<VectorFormatPCT> m_vertexBuffer;
        private readonly IElementBuffer m_elementBuffer;

        private readonly Mesh m_mesh;

        private readonly Device m_device;

        public TestObject(Device device)
        {
            m_device = device;

            /*
            using var reader = new FBXReader(File.OpenRead(FILE));
            m_mesh = reader.Import().FirstOrDefault();

            if (m_mesh == null)
                throw new Exception($"Unable to load mesh.");
            */

            var verts = new VectorFormatPCT[]
            {
                BuildVector(0.5f, 0.5f, 0),
                BuildVector(0.5f,-0.5f, 0),
                BuildVector(-.5f,-0.5f, 0),
                BuildVector(-.5f, 0.5f, 0.5f)
            };

            var indx = new uint[]
            {
                0, 1, 3,
                1, 2, 3
            };

            m_vertexBuffer = m_device.GetVertexBuffer<VectorFormatPCT>(BufferType.Static);
            m_elementBuffer = m_device.GetElementBuffer(BufferType.Static);

            m_vertexBuffer.Set(verts);
            m_elementBuffer.Set(indx);

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

        public override void Render()
        {
            m_vertexBuffer.Activate();
            //m_elementBuffer.Activate();

            //m_device.DrawElements(PrimitiveType.TrangleList, m_mesh.Indicies.Count);

            //m_device.DrawArrays(PrimitiveType.TrangleList, 0, m_mesh.Verts.Count);

            m_device.DrawElements(PrimitiveType.TrangleList, 6);
        }
    }
}
