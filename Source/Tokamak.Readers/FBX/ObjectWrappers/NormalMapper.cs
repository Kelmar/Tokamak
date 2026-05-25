using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

using Tokamak.Mathematics;
using Tokamak.Tritium.Geometry;

namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal class NormalMapper
    {
        private enum MappingType
        {
            None,
            Vertex,
            Polygon,
            PolyVertex,
            Edge
        }

        private readonly Node? m_node;
        private readonly MappingType m_mapType;
        private readonly List<Vector3> m_data = [];

        public NormalMapper(Node? node)
        {
            m_node = node;

            m_mapType = ReadMappingType();
            LoadNormalData();
        }

        private MappingType ReadMappingType()
        {
            Node? mappingTypeNode = m_node?
                .GetChildren("MappingInformationType")
                .FirstOrDefault();

            string mappingTypeStr = mappingTypeNode?.Properties.FirstOrDefault()?.ToString() ?? String.Empty;

            return mappingTypeStr.ToLower() switch
            {
                "byvertice" => MappingType.Vertex, // Blender weirdness
                "byvertex" => MappingType.Vertex,
                "bypolygon" => MappingType.Polygon,
                "bypolygonvertex" => MappingType.PolyVertex,
                "byedge" => MappingType.Edge,

                "allsame" => MappingType.None, // We aren't goign to support this, we'll treat like we didn't get the info.
                _ => MappingType.None
            };
        }

        private void LoadNormalData()
        {
            if (m_mapType == MappingType.None)
                return; // If the type comes back as none, we don't read any normal data, we'll calculate it.

            // Shouldn't be possible to get anything but "None" if we didn't get a node.
            Debug.Assert(m_node != null);

            m_data.AddRange(m_node
                .GetChildren("Normals")
                .SelectMany(n => n.Properties[0].AsEnumerable<float>())
                .ToList()
                .Chunk(3)
                .Select(VectorEx.ToVector3)
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
            switch (m_mapType)
            {
            case MappingType.Polygon:
            case MappingType.None:
            default:
                return; // We calculate the normals in FinalizeNormals()

            case MappingType.Edge:
                return; // We calculate the normals in FinalizeNoramls()

            case MappingType.Vertex:
                polygon.Normals.Add(m_data[index]);
                break;

            case MappingType.PolyVertex:
                polygon.Normals.Add(m_data[indexNo]);
                break;
            }
        }

        public void FinalizeNormals(Polygon polygon)
        {

        }
    }
}
