using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Tokamak.Readers.FBX.Builders;

namespace Tokamak.Readers.FBX.Mappers
{
    internal class MaterialMapper
    {
        private readonly Node? m_node;
        private readonly IndexMapper m_indexMapper;
        private readonly List<int> m_data = [];

        public MaterialMapper(Node? node)
        {
            m_node = node;

            m_indexMapper = new IndexMapper(m_node, "MaterialIndex");

            LoadMaterialData();
        }

        private void LoadMaterialData()
        {
            if (m_indexMapper.MappingType == VertexMappingType.None)
                return; // If the type comes back as none, we don't read any material data.

            // Shouldn't be possible to get anything but "None" if we didn't get a node.
            Debug.Assert(m_node != null);

            m_data.AddRange(m_node.Children
                .WithName("Materials")
                .SelectMany(n => n.Properties[0].AsEnumerable<int>())
                .ToList()                
            );
        }

        public int GetMaterial(int polyIdx, int indexNo, int index)
        {
            int idx = m_indexMapper.MapIndex(polyIdx, indexNo, index);

            if (idx != -1)
                return m_data[idx];

            return 0;
        }
    }
}
