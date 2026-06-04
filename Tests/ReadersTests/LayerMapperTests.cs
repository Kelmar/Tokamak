using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using NUnit.Framework;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Mappers;

using static ReadersTests.Support.NodeBuilder;

namespace ReadersTests
{
    [TestFixture]
    public class LayerMapperTests
    {
        [Test]
        public void GetItem_ResolvesDataThroughMapping()
        {
            var node = Make("LayerElementMaterial", props: null, children: new[]
            {
                StringNode("MappingInformationType", "ByPolygonVertex"),
                StringNode("ReferenceInformationType", "Direct"),
                Make("Materials", new[] { IntArray(7, 8, 9) }),
            });

            var mapper = new LayerMapper<int>(
                node,
                "Materials",
                "MaterialIndex",
                props => props.SelectMany(p => p.AsEnumerable<int>()));

            // GetItem(indexNumber, polyIndex, vectorIndex); PolyVertex uses indexNumber.
            Assert.That(mapper.GetItem(1, 0, 0), Is.EqualTo(8));
        }

        [Test]
        public void GetItem_OutOfRange_ReturnsDefault()
        {
            var node = Make("LayerElementMaterial", props: null, children: new[]
            {
                StringNode("MappingInformationType", "ByPolygonVertex"),
                StringNode("ReferenceInformationType", "Direct"),
                Make("Materials", new[] { IntArray(7, 8, 9) }),
            });

            var mapper = new LayerMapper<int>(
                node,
                "Materials",
                "MaterialIndex",
                props => props.SelectMany(p => p.AsEnumerable<int>()));

            Assert.That(mapper.GetItem(99, 0, 0), Is.EqualTo(0));
        }

        [Test]
        public void NoMappingInformation_YieldsDefault()
        {
            // No MappingInformationType node -> mapping type None -> no data loaded.
            var node = Make("LayerElementUV", props: null, children: new[]
            {
                Make("UV", new[] { DoubleArray(0.5, 0.5) }),
            });

            var mapper = new LayerMapper<Vector2>(
                node,
                "UV",
                "UVIndex",
                props => props.SelectMany(p => p.AsEnumerable<float>()).ToList().Chunk(2)
                              .Select(c => new Vector2(c[0], c.Length > 1 ? c[1] : 0)));

            Assert.That(mapper.GetItem(0, 0, 0), Is.EqualTo(Vector2.Zero));
        }
    }
}
