using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Tokamak.Mathematics;

using Tokamak.Readers.FBX.Readers;

namespace Tokamak.Readers.FBX.Mappers
{
    internal class MapperBase<T>
        where T : struct
    {
        private readonly IndexMapper m_indexMapper;
        private readonly List<T> m_data = [];

        protected MapperBase(Node? node, string childNodeName, string indexName)
        {
            m_indexMapper = new IndexMapper(node, indexName);

            if (m_indexMapper.MappingType == VertexMappingType.None)
                return; // If the type comes back as none, we don't read any data.

            // Shouldn't be possible to get anything but "None" if we didn't get a node.
            Debug.Assert(node != null);

            LoadData(node, childNodeName);
        }

        private void LoadData(Node node, string childNodeName)
        {
            //var props = node.Children
            //    .WithName(childNodeName)
            //    .SelectMany(n => n.Properties[0]);
            

            //.AsEnumerable<float>())
            //    .ToList();

            //m_data.AddRange(props.Chunk(2).Select(VectorEx.ToVector2));
        }

        public T GetItem(int indexNumber, int polyIndex, int vectorIndex)
        {
            int i = m_indexMapper.MapIndex(polyIndex, indexNumber, vectorIndex);

            if (i != -1)
                return m_data[i];

            return default;
        }
    }
}
