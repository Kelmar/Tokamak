using System.Numerics;

using NUnit.Framework;

using Tokamak.Readers.FBX.DOM;

namespace ReadersTests
{
    /// <summary>
    /// Tests that encode the *desired* behavior for the bugs surfaced during the
    /// FBX pipeline analysis.  They are <see cref="IgnoreAttribute"/>'d because they
    /// currently fail against the production code; remove the attribute once the
    /// corresponding fix lands and they become live regression tests.
    /// </summary>
    [TestFixture]
    [Category("KnownIssues")]
    public class KnownIssueTests
    {
        [Test]
        [Ignore("Bug #1: GlobalSettings.MapToVector indexes by axis without bounds checking; " +
                "a short chunk (truncated Vertices array) throws IndexOutOfRangeException.")]
        public void MapToVector_ShortInput_DoesNotThrow()
        {
            var settings = new GlobalSettings();

            Vector3 result = Vector3.Zero;
            Assert.That(() => result = settings.MapToVector([ 1, 2 ]), Throws.Nothing);
            Assert.That(result, Is.EqualTo(new Vector3(1, 2, 0)));
        }
    }
}
