using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Readers;

using ReadersTests.Support;

namespace ReadersTests
{
    [TestFixture]
    public class BinaryFormatReaderTests
    {
        private static Node ReadSingleNode(byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var reader = new BinaryFormatReader(stream, Encoding.UTF8);
            return reader.ReadNode();
        }

        [Test]
        public void ReadNode_ParsesScalarProperties()
        {
            var builder = new FbxBinaryBuilder(7400);

            byte[] bytes = builder.Build(new FbxBinaryBuilder.NodeSpec
            {
                Name = "Test",
                Props =
                {
                    FbxBinaryBuilder.Int(42),
                    FbxBinaryBuilder.Double(3.5),
                    FbxBinaryBuilder.Str("hello"),
                }
            });

            Node node = ReadSingleNode(bytes);

            Assert.That(node, Is.Not.Null);
            Assert.That(node.Name, Is.EqualTo("Test"));
            Assert.That(node.Properties, Has.Count.EqualTo(3));
            Assert.That(node.Properties[0].AsInt(), Is.EqualTo(42));
            Assert.That((double)node.Properties[1].Data, Is.EqualTo(3.5).Within(1e-9));
            Assert.That(node.Properties[2].AsString(), Is.EqualTo("hello"));
        }

        [Test]
        public void ReadNode_DecodesClassDelimiterAsDoubleColon()
        {
            // Binary FBX encodes a "name::class" separator as 0x00 0x01.
            var builder = new FbxBinaryBuilder(7400);
            byte[] bytes = builder.Build(new FbxBinaryBuilder.NodeSpec
            {
                Name = "Test",
                Props = { FbxBinaryBuilder.Str("Geometry\0\x01Mesh") }
            });

            Node node = ReadSingleNode(bytes);

            Assert.That(node.Properties[0].AsString(), Is.EqualTo("Geometry::Mesh"));
        }

        [Test]
        public void ReadNode_ParsesUncompressedArrays()
        {
            var builder = new FbxBinaryBuilder(7400);
            byte[] bytes = builder.Build(new FbxBinaryBuilder.NodeSpec
            {
                Name = "Vertices",
                Props =
                {
                    FbxBinaryBuilder.IntArray(1, 2, 3, 4),
                    FbxBinaryBuilder.DoubleArray(1.5, 2.5, 3.5),
                }
            });

            Node node = ReadSingleNode(bytes);

            Assert.That(node.Properties[0].AsEnumerable<int>().ToArray(), Is.EqualTo(new[] { 1, 2, 3, 4 }));
            Assert.That(node.Properties[1].AsEnumerable<double>().ToArray(),
                Is.EqualTo(new[] { 1.5, 2.5, 3.5 }).Within(1e-9));
        }

        [Test]
        public void ReadNode_ParsesNestedChildren()
        {
            var builder = new FbxBinaryBuilder(7400);
            byte[] bytes = builder.Build(new FbxBinaryBuilder.NodeSpec
            {
                Name = "Parent",
                Props = { FbxBinaryBuilder.Int(1) },
                Children =
                {
                    new FbxBinaryBuilder.NodeSpec { Name = "ChildA", Props = { FbxBinaryBuilder.Int(10) } },
                    new FbxBinaryBuilder.NodeSpec { Name = "ChildB", Props = { FbxBinaryBuilder.Int(20) } },
                }
            });

            Node node = ReadSingleNode(bytes);

            Assert.That(node.Name, Is.EqualTo("Parent"));
            Assert.That(node.Children, Has.Count.EqualTo(2));
            Assert.That(node.Children[0].Name, Is.EqualTo("ChildA"));
            Assert.That(node.Children[1].Properties[0].AsInt(), Is.EqualTo(20));
        }

        [TestCase(7400u)] // 32-bit node lengths
        [TestCase(7500u)] // 64-bit node lengths
        public void ReadNode_HandlesBothVersionLengthEncodings(uint version)
        {
            var builder = new FbxBinaryBuilder(version);
            byte[] bytes = builder.Build(new FbxBinaryBuilder.NodeSpec
            {
                Name = "Versioned",
                Props = { FbxBinaryBuilder.Int(7) }
            });

            Node node = ReadSingleNode(bytes);

            Assert.That(node.Name, Is.EqualTo("Versioned"));
            Assert.That(node.Properties[0].AsInt(), Is.EqualTo(7));
        }
    }
}
