using System.Collections.Generic;

using NUnit.Framework;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Mappers;

using static ReadersTests.Support.NodeBuilder;

namespace ReadersTests
{
    [TestFixture]
    public class IndexMapperTests
    {
        private static Node LayerNode(string mapping, string reference, string indexName = null, int[] indices = null)
        {
            var children = new List<Node>
            {
                StringNode("MappingInformationType", mapping),
                StringNode("ReferenceInformationType", reference),
            };

            if (indexName != null && indices != null)
                children.Add(Make(indexName, new[] { IntArray(indices) }));

            return Make("LayerElementNormal", props: null, children: children);
        }

        [Test]
        public void ReadsMappingAndReferenceTypes()
        {
            var mapper = new IndexMapper(LayerNode("ByPolygonVertex", "Direct"), "NormalsIndex");

            Assert.That(mapper.MappingType, Is.EqualTo(VertexMappingType.PolyVertex));
            Assert.That(mapper.ReferenceType, Is.EqualTo(VertexReferenceType.Direct));
        }

        [TestCase("ByVertice", VertexMappingType.Vertex)]   // Blender spelling
        [TestCase("ByVertex", VertexMappingType.Vertex)]
        [TestCase("ByPolygon", VertexMappingType.Polygon)]
        [TestCase("ByPolygonVertex", VertexMappingType.PolyVertex)]
        [TestCase("AllSame", VertexMappingType.AllSame)]
        [TestCase("garbage", VertexMappingType.None)]
        public void MappingType_IsParsedCaseInsensitively(string text, object expected)
        {
            var mapper = new IndexMapper(LayerNode(text, "Direct"), "NormalsIndex");
            Assert.That(mapper.MappingType, Is.EqualTo(expected));
        }

        [Test]
        public void MapIndex_AllSame_AlwaysReturnsZero()
        {
            var mapper = new IndexMapper(LayerNode("AllSame", "Direct"), "NormalsIndex");

            Assert.That(mapper.MapIndex(5, 7, 9), Is.EqualTo(0));
        }

        [Test]
        public void MapIndex_ByVertexDirect_ReturnsVectorIndex()
        {
            var mapper = new IndexMapper(LayerNode("ByVertex", "Direct"), "NormalsIndex");

            // signature is MapIndex(polyIdx, indexNo, index)
            Assert.That(mapper.MapIndex(1, 2, 3), Is.EqualTo(3));
        }

        [Test]
        public void MapIndex_ByPolygonVertexDirect_ReturnsRunningIndex()
        {
            var mapper = new IndexMapper(LayerNode("ByPolygonVertex", "Direct"), "NormalsIndex");

            Assert.That(mapper.MapIndex(1, 2, 3), Is.EqualTo(1));
        }

        [Test]
        public void MapIndex_IndexToDirect_LooksUpThroughIndexTable()
        {
            var mapper = new IndexMapper(
                LayerNode("ByPolygonVertex", "IndexToDirect", "NormalsIndex", [ 10, 20, 30 ]),
                "NormalsIndex");

            Assert.That(mapper.ReferenceType, Is.EqualTo(VertexReferenceType.IndexToDirect));

            // running index 1 -> indices[1] == 20
            Assert.That(mapper.MapIndex(0, 1, 0), Is.EqualTo(10));
        }
    }
}
