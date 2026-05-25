using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

using Tokamak.Tritium.Geometry;

namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal class NormalMapper
    {
        private readonly Node? m_node;
        private readonly GlobalSettings m_settings;
        private readonly IndexMapper m_indexMapper;
        private readonly List<Vector3> m_data = [];

        public NormalMapper(GlobalSettings settings, Node? node)
        {
            m_node = node;
            m_settings = settings;

            m_indexMapper = new IndexMapper(m_node, "NormalsIndex");

            LoadNormalData();
        }

        private void LoadNormalData()
        {
            if (m_indexMapper.MappingType == VertexMappingType.None)
                return; // If the type comes back as none, we don't read any normal data, we'll calculate it.

            // Shouldn't be possible to get anything but "None" if we didn't get a node.
            Debug.Assert(m_node != null);

            m_data.AddRange(m_node
                .GetChildren("Normals")
                .SelectMany(n => n.Properties[0].AsEnumerable<float>())
                .ToList()
                .Chunk(3)
                .Select(m_settings.MapToVector)
            );
        }

        /// <summary>
        /// Add vertex normal to polygon.
        /// </summary>
        /// <param name="polygon">The polygon to add the normal to.</param>
        /// <param name="indexNo">The index of the index to use if needed.</param>
        /// <param name="index">The vertex index to add the normal for.</param>
        public void AddNormal(Polygon polygon, int indexNo, int index)
        {
            int i = m_indexMapper.MapIndex(indexNo, index);

            if (i != -1)
                polygon.Normals.Add(m_data[i]);
        }

        public void FinalizeNormals(Polygon polygon)
        {

        }
    }
}
