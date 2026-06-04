using System.Numerics;

using NUnit.Framework;

using Tokamak.Readers.FBX.DOM;

namespace ReadersTests
{
    [TestFixture]
    public class GlobalSettingsTests
    {
        [Test]
        public void SwizzleAxes_WithDefaultAxes_IsIdentity()
        {
            var settings = new GlobalSettings();

            Vector3 result = settings.SwizzleAxes(new Vector3(1, 2, 3));

            Assert.That(result, Is.EqualTo(new Vector3(1, 2, 3)));
        }

        [Test]
        public void SwizzleAxes_RemapsAxesAndAppliesSigns()
        {
            // Up=Z, Front=Y style remap with a negated coord axis.
            var settings = new GlobalSettings
            {
                CoordAxis = 0,
                CoordAxisSign = -1,
                UpAxis = 2,
                UpAxisSign = 1,
                FrontAxis = 1,
                FrontAxisSign = 1,
            };

            settings.BuildSwizzleMatrix();

            Vector3 result = settings.SwizzleAxes(new Vector3(1, 2, 3));

            // x = coord(values[0]) * -1, y = up(values[2]), z = front(values[1])
            Assert.That(result, Is.EqualTo(new Vector3(-1, 3, 2)));
        }
    }
}
