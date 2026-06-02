using System.Collections.Generic;
using System.Linq;

using Tokamak.Assets;
using Tokamak.Mathematics;

using Tokamak.Tritium.APIs;
using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;

namespace Tokamak.Tritium.Geometry
{
    public class Mesh : Asset
    {
        private readonly IVertexBuffer<VectorFormatPNCT> m_vertexBuffer;
        private readonly IElementBuffer m_elementBuffer;

        public Mesh(IGraphicsLayer graphicsLayer)
        {
            m_vertexBuffer = graphicsLayer.GetVertexBuffer<VectorFormatPNCT>(BufferUsage.Static);
            m_elementBuffer = graphicsLayer.GetElementBuffer(BufferUsage.Static);

            IndexCount = 0;
        }

        public bool IsEmpty => IndexCount == 0;

        public int IndexCount { get; private set; }

        public void SetData(IEnumerable<Polygon> polygons)
        {
            var polyData = polygons
                .SelectMany(p => p.SplitIntoTriangles())
                .ToList();

            // Dictionary is used to filter out duplicate vectors.
            int preAllocate = polyData.Sum(p => p.Vectors.Count);
            var vectorFilter = new Dictionary<VectorFormatPNCT, uint>(preAllocate);

            var indexList = new List<uint>(preAllocate);

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

                    indexList.Add(index);
                }
            }

            m_vertexBuffer.Set(vectorFilter.Keys.ToArray());
            m_elementBuffer.Set(indexList.ToArray());

            IndexCount = indexList.Count;
        }

        private IEnumerable<VectorFormatPNCT> ToVectorFormat(Polygon poly)
        {
            for (int i = 0; i < poly.Vectors.Count; ++i)
            {
                yield return new VectorFormatPNCT
                {
                    Point = poly.Vectors[i],
                    Color = poly.Colors[i],
                    Normal = poly.Normals[i],
                    TexCoord = poly.TexCoord[i]
                };
            }
        }

        public void Draw(ICommandList commandList)
        {
            m_elementBuffer.Activate();
            m_vertexBuffer.Activate();

            commandList.DrawElements(IndexCount);
        }
    }
}
