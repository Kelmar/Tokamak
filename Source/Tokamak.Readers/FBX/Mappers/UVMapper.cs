using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Readers.FBX.Mappers
{
    internal class UVMapper
    {
        private readonly Node? m_node;
        private readonly IndexMapper m_indexMapper;
        private readonly List<Vector2> m_data = [];

        public UVMapper(Node? node)
        {
            m_node = node;

            m_indexMapper = new IndexMapper(m_node, "UVIndex");

            LoadUVData();
        }

        private void LoadUVData()
        {
            if (m_indexMapper.MappingType == VertexMappingType.None)
                return; // If the type comes back as none, we don't read any UV data.

            // Shouldn't be possible to get anything but "None" if we didn't get a node.
            Debug.Assert(m_node != null);

            m_data.AddRange(m_node
                .Children["UV"]
                .SelectMany(n => n.Properties[0].AsEnumerable<float>())
                .ToList()
                .Chunk(2)
                .Select(VectorEx.ToVector2)
            );
        }

        public Vector2 GetUV(int polyIdx, int indexNo, int index)
        {
            int i = m_indexMapper.MapIndex(polyIdx, indexNo, index);

            if (i != -1)
                return m_data[i];

            return Vector2.Zero;
        }
    }
}
