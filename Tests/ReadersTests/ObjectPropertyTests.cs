using System;
using System.Numerics;

using NUnit.Framework;

using Tokamak.Readers.FBX.DOM;

using static ReadersTests.Support.NodeBuilder;

namespace ReadersTests
{
    [TestFixture]
    public class ObjectPropertyTests
    {
        // Properties70 "P" nodes are laid out as:
        //   [name, type, label, flags, value0, value1, ...]
        private static Node P(string name, string type, params NodeProperty[] values)
        {
            var props = new System.Collections.Generic.List<NodeProperty>
            {
                Str(name), Str(type), Str(""), Str(""),
            };
            props.AddRange(values);
            return Make("P", props);
        }

        [Test]
        public void Build_IntProperty_ReadsTrailingValue()
        {
            var op = ObjectProperty.Build(P("MyInt", "int", Int(42)));

            Assert.That(op, Is.Not.Null);
            Assert.That(op.Name, Is.EqualTo("MyInt"));
            Assert.That(op.Type, Is.EqualTo("int"));
            Assert.That(Convert.ToInt32(op.Data), Is.EqualTo(42));
        }

        [Test]
        public void Build_Vector3Property_ReadsLastThreeValues()
        {
            var op = ObjectProperty.Build(P("Pos", "Vector3D", Dbl(1.0), Dbl(2.0), Dbl(3.0)));

            Assert.That(op, Is.Not.Null);
            Assert.That(op.Data, Is.TypeOf<Vector3>());
            Assert.That((Vector3)op.Data, Is.EqualTo(new Vector3(1, 2, 3)));
        }

        [Test]
        public void Build_ColorProperty_ReadsLastFourValuesAsVector4()
        {
            var op = ObjectProperty.Build(P("DiffuseColor", "Color", Dbl(0.1), Dbl(0.2), Dbl(0.3), Dbl(1.0)));

            Assert.That(op, Is.Not.Null);
            Assert.That(op.Data, Is.TypeOf<Vector4>());

            var v = (Vector4)op.Data;
            Assert.That(v.X, Is.EqualTo(0.1f).Within(1e-5));
            Assert.That(v.Y, Is.EqualTo(0.2f).Within(1e-5));
            Assert.That(v.Z, Is.EqualTo(0.3f).Within(1e-5));
            Assert.That(v.W, Is.EqualTo(1.0f).Within(1e-5));
        }

        [Test]
        public void Build_UnknownType_ReturnsNull()
        {
            Assert.That(ObjectProperty.Build(P("Mystery", "someWeirdType", Int(1))), Is.Null);
        }

        // The following exercise ReadValues against malformed/short vector properties.
        // They must terminate (a regressed loop here would hang) and zero-fill the
        // missing components rather than throwing or mis-aligning.

        [Test]
        public void Build_VectorProperty_WithNoValues_ReturnsZeroVector()
        {
            // [name, type, label, flags] only - no numeric components.
            var op = ObjectProperty.Build(P("DiffuseColor", "Color"));

            Assert.That(op, Is.Not.Null);
            Assert.That((Vector4)op.Data, Is.EqualTo(Vector4.Zero));
        }

        [Test]
        public void Build_VectorProperty_WithTooFewProperties_ReturnsZeroVector()
        {
            // Truncated P node: only name + type, so the "count from the end" start
            // index goes negative and must be clamped instead of crashing or hanging.
            var node = Make("P", new[] { Str("DiffuseColor"), Str("Color") });

            var op = ObjectProperty.Build(node);

            Assert.That(op, Is.Not.Null);
            Assert.That((Vector4)op.Data, Is.EqualTo(Vector4.Zero));
        }

        [Test]
        public void Build_VectorProperty_WithPartialValues_PadsRemainderWithZero()
        {
            // Vector3D with only two numeric components present; the third pads to 0.
            var op = ObjectProperty.Build(P("Pos", "Vector3D", Dbl(1.0), Dbl(2.0)));

            Assert.That(op, Is.Not.Null);
            Assert.That((Vector3)op.Data, Is.EqualTo(new Vector3(1, 2, 0)));
        }
    }
}
