using NUnit.Framework;

using Tokamak.Import.FBX.DOM;

using static ImportFBXTests.Support.NodeBuilder;

namespace ImportFBXTests
{
    [TestFixture]
    public class FBXObjectTests
    {
        // The ObjectGraph is only stored for lazy child/parent lookups; the
        // constructor never dereferences it, so null is fine for these tests.
        private static FBXObject Build(Node node)
        {
            // TODO: Fix this null here.
            return new FBXObject(null!, node);
        }

        [Test]
        public void ParsesId_Name_Class_AndSubClass()
        {
            var node = MakeNode("Geometry", new[]
            {
                Long(12345),
                Str("myMesh\0\x01Geometry".Replace("\0\x01", "::")), // already-resolved binary delimiter
                Str("Mesh"),
            });

            var obj = Build(node);

            Assert.That(obj.Id, Is.EqualTo(12345));
            Assert.That(obj.Name, Is.EqualTo("myMesh"));
            Assert.That(obj.Class, Is.EqualTo("Geometry"));
            Assert.That(obj.SubClass, Is.EqualTo("Mesh"));
            Assert.That(obj.Type, Is.EqualTo("Geometry")); // Type is the node name.
        }

        [Test]
        public void Name_WithoutClassDelimiter_LeavesClassEmpty()
        {
            var node = MakeNode("Model", new[] { Long(1), Str("justAName"), Str("Light") });

            var obj = Build(node);

            Assert.That(obj.Name, Is.EqualTo("justAName"));
            Assert.That(obj.Class, Is.Empty);
            Assert.That(obj.SubClass, Is.EqualTo("Light"));
        }

        [Test]
        public void MissingProperties_FallBackToDefaults()
        {
            var node = MakeNode("Empty");

            var obj = Build(node);

            Assert.That(obj.Id, Is.EqualTo(-1));
            Assert.That(obj.Name, Is.Empty);
            Assert.That(obj.Class, Is.Empty);
            Assert.That(obj.SubClass, Is.Empty);
        }
    }
}
