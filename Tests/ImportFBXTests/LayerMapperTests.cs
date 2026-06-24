using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using NUnit.Framework;

using Tokamak.Import.FBX.DOM;
using Tokamak.Import.FBX.Mappers;

using static ImportFBXTests.Support.NodeBuilder;

namespace ImportFBXTests
{
    [TestFixture]
    public class LayerMapperTests
    {
        private static Node MakeMaterialLayer()
        {
            return MakeNode("LayerElementMaterial", props: null, children:
            [
                StringNode("MappingInformationType", "ByPolygonVertex"),
                StringNode("ReferenceInformationType", "Direct"),
                MakeNode("Materials", [IntArray(7, 8, 9)]),
            ]);
        }

        private static Node MakeUVLayer()
        {
            return MakeNode("LayerElementUV", props: null, children:
            [
                MakeNode("UV", [ DoubleArray(0.5, 0.5) ]),
            ]);
        }

        private static Node MakeTestMeshNode()
        {
            return MakeNode("Mesh", props: null, children:
            [
                StringNode("Foo", "Bar"),
                MakeMaterialLayer(),
                MakeUVLayer()
            ]);
        }

        [Test]
        public void GetItem_ResolvesDataThroughMapping()
        {
            var testMesh = MakeTestMeshNode();

            var mapper = new LayerMapper<int>(
                testMesh.Children,
                "LayerElementMaterial",
                "Materials",
                "MaterialIndex",
                props => props.SelectMany(p => p.AsEnumerable<int>()));

            // GetItem(indexNumber, polyIndex, vectorIndex); PolyVertex uses indexNumber.
            Assert.That(mapper.GetItem(1, 0, 0), Is.EqualTo(8));
        }

        [Test]
        public void GetItem_OutOfRange_ReturnsDefault()
        {
            var testMesh = MakeTestMeshNode();

            var mapper = new LayerMapper<int>(
                testMesh.Children,
                "LayerElementMaterial",
                "Materials",
                "MaterialIndex",
                props => props.SelectMany(p => p.AsEnumerable<int>()));

            Assert.That(mapper.GetItem(99, 0, 0), Is.Zero);
        }

        [Test]
        public void NoMappingInformation_YieldsDefault()
        {
            // No MappingInformationType node -> mapping type None -> no data loaded.
            var testMesh = MakeTestMeshNode();

            var mapper = new LayerMapper<Vector2>(
                testMesh.Children,
                "LayerElementUV",
                "UV",
                "UVIndex",
                props => props.SelectMany(p => p.AsEnumerable<float>()).ToList().Chunk(2)
                              .Select(c => new Vector2(c[0], c.Length > 1 ? c[1] : 0)));

            Assert.That(mapper.GetItem(0, 0, 0), Is.EqualTo(Vector2.Zero));
        }
    }
}
