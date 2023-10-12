using System;
using System.Numerics;

using Tokamak;
using Tokamak.Buffer;
using Tokamak.Formats;

namespace TestBed
{
    public class TestObject : IDisposable
    {
        private readonly IVertexBuffer<VectorFormatPCT> m_vertexBuffer;
        private readonly Device m_device;

        public TestObject(Device device)
        {
            m_device = device;

            m_vertexBuffer = m_device.GetVertexBuffer<VectorFormatPCT>(BufferType.Static);

            var data = new VectorFormatPCT[4]
            {
                BuildVector( 5f, 5f, 0),
                BuildVector( 5f,-5f, 0),
                BuildVector(-5f, 5f, 0),
                BuildVector(-5f,-5f, 0)
            };

            m_vertexBuffer.Set(data);
        }

        public void Dispose()
        {
            m_vertexBuffer.Dispose();
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

        public void Render()
        {
            m_vertexBuffer.Activate();
            m_device.DrawArrays(PrimitiveType.TrangleStrip, 0, 4);
        }
    }
}
