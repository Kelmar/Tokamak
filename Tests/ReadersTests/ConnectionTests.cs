using NUnit.Framework;

using Tokamak.Readers.FBX.Mappers;

using ReadersTests.Support;

using static ReadersTests.Support.NodeBuilder;

namespace ReadersTests
{
    [TestFixture]
    public class ConnectionTests
    {
        [Test]
        public void Build_WithThreeProperties_ParsesTypeFromToAndUppercasesType()
        {
            var node = Make("C", new[] { Str("oo"), Long(100), Long(200) });

            var connection = Connection.Build(node);

            Assert.That(connection, Is.Not.Null);
            Assert.That(connection.Type, Is.EqualTo("OO"));
            Assert.That(connection.From, Is.EqualTo(100));
            Assert.That(connection.To, Is.EqualTo(200));
            Assert.That(connection.PropertyName, Is.Empty);
        }

        [Test]
        public void Build_WithFourProperties_ReadsPropertyName()
        {
            var node = Make("C", new[] { Str("OP"), Long(1), Long(2), Str("DiffuseColor") });

            var connection = Connection.Build(node);

            Assert.That(connection, Is.Not.Null);
            Assert.That(connection.Type, Is.EqualTo("OP"));
            Assert.That(connection.PropertyName, Is.EqualTo("DiffuseColor"));
        }

        [Test]
        public void Build_WithFewerThanThreeProperties_ReturnsNull()
        {
            var node = Make("C", new[] { Str("OO"), Long(1) });

            Assert.That(Connection.Build(node), Is.Null);
        }
    }
}
