using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal class IndexMapper
    {
        private readonly Node? m_node;
        private readonly List<int> m_indices = [];

        public IndexMapper(Node? node, string indexName)
        {
            m_node = node;

            MappingType = ReadMappingType();
            ReferenceType = ReadReferenceType();

            LoadIndices(indexName);
        }

        public VertexMappingType MappingType { get; }

        public VertexReferenceType ReferenceType { get; }

        private string ReadNodeAsString(string nodeName)
        {
            Node? node = m_node?.GetChildren(nodeName).FirstOrDefault();

            return node?.Properties.FirstOrDefault()?.ToString() ?? String.Empty;
        }

        private VertexMappingType ReadMappingType()
        {
            string mappingTypeStr = ReadNodeAsString("MappingInformationType");

            return mappingTypeStr.ToLower() switch
            {
                "byvertice" => VertexMappingType.Vertex, // Blender weirdness
                "byvertex" => VertexMappingType.Vertex,
                "bypolygon" => VertexMappingType.Polygon,
                "bypolygonvertex" => VertexMappingType.PolyVertex,
                "byedge" => VertexMappingType.Edge,

                "allsame" => VertexMappingType.None, // We aren't going to support this, we'll treat like we didn't get the info.
                _ => VertexMappingType.None
            };
        }

        private VertexReferenceType ReadReferenceType()
        {
            string referenceTypeStr = ReadNodeAsString("ReferenceInformationType");

            return referenceTypeStr.ToLower() switch
            {
                "indextodirect" => VertexReferenceType.Index, // Blender weirdness
                "direct" => VertexReferenceType.Direct,
                _ => VertexReferenceType.Direct
            };
        }

        private void LoadIndices(string indexName)
        {
            if (ReferenceType == VertexReferenceType.Direct)
                return; // If the type comes back as none, we don't read any normal data, we'll calculate it.

            // Shouldn't be possible to get anything but "None" if we didn't get a node.
            Debug.Assert(m_node != null);

            m_indices.AddRange(m_node
                .GetChildren(indexName)
                .SelectMany(n => n.Properties[0].AsEnumerable<int>())
            );
        }

        public int MapIndex(int indexNo, int index)
        {
            int i = MappingType switch
            {
                VertexMappingType.Vertex => index,
                VertexMappingType.PolyVertex => indexNo,

                VertexMappingType.Polygon => -1,
                VertexMappingType.None => -1,
                VertexMappingType.Edge => -1,
                _ => -1
            };

            if (i != -1 && ReferenceType == VertexReferenceType.Index)
                return m_indices[i];

            return i;
        }
    }
}
