using System.Collections.Generic;
using System.Numerics;

using NUnit.Framework;

using Tokamak.Readers.FBX.DOM;
using Tokamak.Readers.FBX.Mappers;

using static ReadersTests.Support.NodeBuilder;

namespace ReadersTests
{
    [TestFixture]
    public class ObjectMapperTests
    {
        private static ObjectProperty BuildProp(string name, string type, params NodeProperty[] values)
        {
            var props = new List<NodeProperty> { Str(name), Str(type), Str(""), Str("") };
            props.AddRange(values);
            return ObjectProperty.Build(Make("P", props));
        }

        [Test]
        public void MapTo_MapsScalarAndVectorProperties()
        {
            var props = new[]
            {
                BuildProp("DiffuseColor", "Color", Dbl(0.1), Dbl(0.2), Dbl(0.3), Dbl(1.0)),
                BuildProp("AmbientFactor", "double", Dbl(0.5)),
            };

            var material = props.MapTo<MaterialInfo>();

            Assert.That(material.AmbientFactor, Is.EqualTo(0.5f).Within(1e-5));

            var c = material.DiffuseColor;
            Assert.That(c.X, Is.EqualTo(0.1f).Within(1e-5));
            Assert.That(c.W, Is.EqualTo(1.0f).Within(1e-5));
        }

        [Test]
        public void MapTo_RespectsNotMappedAttribute()
        {
            // ShadingModel is [NotMapped] on MaterialInfo, so the mapper must ignore it
            // and leave the default value in place.
            var props = new[]
            {
                BuildProp("ShadingModel", "string", Str("blinn")),
            };

            var material = props.MapTo<MaterialInfo>();

            Assert.That(material.ShadingModel, Is.EqualTo("phong"));
        }

        [Test]
        public void MapTo_UnknownProperties_AreIgnored()
        {
            var props = new[]
            {
                BuildProp("SomePropertyThatDoesNotExist", "double", Dbl(99)),
            };

            // Should simply produce a default-initialized instance without throwing.
            var material = props.MapTo<MaterialInfo>();

            Assert.That(material, Is.Not.Null);
            Assert.That(material.DiffuseColor, Is.EqualTo(Vector4.One));
        }
    }
}
