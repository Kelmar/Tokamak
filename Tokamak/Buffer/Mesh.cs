using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Formats;

namespace Tokamak.Buffer
{
    public class Mesh : IDisposable
    {
        private List<Vector3> m_verts = new List<Vector3>();
        private List<uint> m_indices = new List<uint>();

        virtual public void Dispose()
        {
        }

        public List<Vector3> Verts 
        {
            get => m_verts;
            set => m_verts = value ?? new List<Vector3>();
        }

        public List<uint> Indicies 
        {
            get => m_indices;
            set => m_indices = value ?? new List<uint>();
        }

        public void ToVertexBuffer(IVertexBuffer<VectorFormatPCT> buffer)
        {
            buffer.Set(
                Verts.Select(v => new VectorFormatPCT
                {
                    Point = v,
                    Color = (Vector4)Color.White,
                    TexCoord = Vector2.Zero
                }));
        }

        public void ToElementsBuffer(IElementBuffer buffer)
        {
            buffer.Set(Indicies.ToArray());
        }
    }
}
