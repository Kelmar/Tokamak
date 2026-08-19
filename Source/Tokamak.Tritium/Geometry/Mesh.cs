using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

using Tokamak.Assets;
using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;

namespace Tokamak.Tritium.Geometry
{
    public class Mesh<TFormat> : Asset
        where TFormat : unmanaged
    {
        private readonly IVertexBuffer<TFormat> m_vertexBuffer;
        private readonly IElementBuffer m_elementBuffer;

        public Mesh(IGraphicsLayer graphicsLayer)
        {
            m_vertexBuffer = graphicsLayer.GetVertexBuffer<TFormat>(BufferUsage.Static);
            m_elementBuffer = graphicsLayer.GetElementBuffer(BufferUsage.Static);

            IndexCount = 0;
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

        public bool IsEmpty => IndexCount == 0;

        public int IndexCount { get; private set; }

        public void SetData(IEnumerable<TFormat> vectors, ReadOnlySpan<uint> indices)
        {
            var indexList = indices.ToArray();

            m_vertexBuffer.Set(vectors.ToArray());
            m_elementBuffer.Set(indexList);

            IndexCount = indexList.Length;
        }

        public void Draw(ICommandList commandList)
        {
            m_elementBuffer.Activate();
            m_vertexBuffer.Activate();

            commandList.DrawElements(IndexCount);
        }
    }

    public class Mesh : Mesh<VertexFormatPNCT>
    {
        public Mesh(IGraphicsLayer graphicsLayer)
            : base(graphicsLayer)
        {
        }

        public void SetData(IEnumerable<Polygon> polygons)
        {
            var polyData = polygons
                .SelectMany(p => p.SplitIntoTriangles())
                .ToList();

            // Dictionary is used to filter out duplicate vectors.
            int preAllocate = polyData.Sum(p => p.Vectors.Count);
            var vectorFilter = new Dictionary<VertexFormatPNCT, uint>(preAllocate);
            
            // TODO: Double check the resulting vert counts after splitting into triangles.
            // TODO: (ALSO ALSO, maybe rework this whole function so that verts aren't directly stored in polys)

            using var lease = MemoryPool<uint>.Shared.Rent(preAllocate);
            var indexList = lease.Memory;
            int i = 0;

            foreach (var poly in polygons)
            {
                var items = ToVectorFormat(poly).ToList();

                foreach (var item in items)
                {
                    if (!vectorFilter.TryGetValue(item, out uint index))
                    {
                        // Add new index to the list
                        index = (uint)vectorFilter.Count;
                        vectorFilter[item] = index;
                    }

                    indexList.Span[i++] = index;
                }
            }

            SetData(vectorFilter.Keys, indexList.Span.Slice(0, i));
        }

        private IEnumerable<VertexFormatPNCT> ToVectorFormat(Polygon poly)
        {
            for (int i = 0; i < poly.Vectors.Count; ++i)
            {
                yield return new VertexFormatPNCT
                {
                    Point = poly.Vectors[i],
                    Color = poly.Colors[i],
                    Normal = poly.Normals[i],
                    TexCoord = poly.TexCoord[i]
                };
            }
        }
    }
}
