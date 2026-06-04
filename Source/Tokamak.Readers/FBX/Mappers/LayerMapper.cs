using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Tokamak.Readers.FBX.DOM;

namespace Tokamak.Readers.FBX.Mappers
{
    internal class LayerMapper<T>
        where T : struct
    {
        private readonly Node? m_node;
        private readonly string m_childNodeName;
        private readonly IndexMapper m_indexMapper;
        private readonly List<T> m_data = [];

        public LayerMapper(
            Node? node,
            string childNodeName,
            string indexName, 
            Func<IEnumerable<NodeProperty>, IEnumerable<T>> dataLoader)
        {
            m_node = node;
            m_childNodeName = childNodeName;

            m_indexMapper = new IndexMapper(m_node, indexName);

            if (m_indexMapper.MappingType == VertexMappingType.None)
                return; // If the type comes back as none, we don't read any data.

            // Shouldn't be possible to get anything but "None" if we didn't get a node.
            Debug.Assert(m_node != null);

            var props = m_node.Children
                .WithName(m_childNodeName)
                .Where(n => n.Properties.Count > 0)
                .Select(n => n.Properties[0]);

            m_data = dataLoader(props).ToList();
        }

        /// <summary>
        /// Fetch mapped item
        /// </summary>
        /// <param name="indexNumber">The index number currently being fetched.</param>
        /// <param name="polyIndex">The index of the polygon the item is being fetched for.</param>
        /// <param name="vectorIndex">The index of the vector being fetched.</param>
        /// <returns>The resulting item or a default value.</returns>
        public T GetItem(int indexNumber, int polyIndex, int vectorIndex)
        {
            int i = m_indexMapper.MapIndex(indexNumber, polyIndex, vectorIndex);

            if (i >= 0 && i < m_data.Count)
                return m_data[i];

            return default;
        }
    }
}
