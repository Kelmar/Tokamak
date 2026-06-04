using System;
using System.Numerics;

using NUnit.Framework;

using Tokamak.Readers.FBX.DOM;

namespace ReadersTests
{
    [TestFixture]
    public class GlobalSettingsTests
    {
        [Test]
        public void Validate_WithDefaultAxes_DoesNotThrow()
        {
            var settings = new GlobalSettings();

            Assert.That(() => settings.Validate(), Throws.Nothing);
        }

        // Valid axis indices are 0..2 (X, Y, Z); anything else must be rejected.
        [TestCase(3)]   // one past the end - would otherwise mis-write the W lane
        [TestCase(4)]
        [TestCase(-1)]
        public void Validate_WithOutOfRangeAxis_Throws(int badAxis)
        {
            var settings = new GlobalSettings { UpAxis = badAxis };

            Assert.That(() => settings.Validate(), Throws.TypeOf<Exception>());
        }

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
