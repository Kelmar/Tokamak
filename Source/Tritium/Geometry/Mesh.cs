using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

using Tokamak.Tritium.Buffers;
using Tokamak.Tritium.Buffers.Formats;

namespace Tokamak.Tritium.Geometry
{
    public class Mesh
    {
        private List<Polygon> m_polygons;

        public List<Polygon> Polygons
        {
            get => m_polygons;
            set => m_polygons = value?.SelectMany(p => p.SplitIntoTriangles()).ToList() ?? new();
        }

        public int ToBuffer(IVertexBuffer<VectorFormatPNCT> vertexBuffer, IElementBuffer elementBuffer)
        {
            // Dictionary is used to filter out duplicate vectors.
            int preAllocate = m_polygons.Select(p => p.Vectors.Count).Sum();
            var vectorFilter = new Dictionary<VectorFormatPNCT, uint>(preAllocate);

            var indexList = new List<uint>(preAllocate);

            foreach (var poly in m_polygons)
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

            vertexBuffer.Set(vectorFilter.Keys.ToArray());
            elementBuffer.Set(indexList.ToArray());

            return indexList.Count;
        }

        private IEnumerable<VectorFormatPNCT> ToVectorFormat(Polygon poly)
        {
            for (int i = 0; i < poly.Vectors.Count; ++i)
            {
                yield return new VectorFormatPNCT
                {
                    Point = poly.Vectors[i],
                    //Color = Color.White.ToVector(),
                    //Color = Color.Brown.ToVector(),
                    Color = Color.Beige.ToVector(),
                    Normal = poly.Normals[i],
                    TexCoord = poly.TexCoord[i]
                };
            }
        }
    }
}
