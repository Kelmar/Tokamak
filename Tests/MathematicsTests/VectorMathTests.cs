using System.Numerics;

using NUnit.Framework;

using Tokamak.Mathematics;

namespace MathTests
{
    public class VectorMathTests
    {
        [Test]
        public void LineSlopeVerticalIsInfinite()
        {
            Vector2 v = new Vector2(0, 1);

            float res = Vector2.Slope(Vector2.Zero, v);

            Assert.That(float.IsInfinity(res));
        }

        [TestCase(1, 0, 0)]
        [TestCase(1, 1, 1)]
        [TestCase(2, 1, 0.5f)]
        [TestCase(1, 2, 2)]
        public void LineSlopeWorks(float x, float y, float expected)
        {
            Vector2 v = new Vector2(x, y);

            float res = Vector2.Slope(Vector2.Zero, v);

            Assert.That(res, Is.EqualTo(expected));
        }

        [Test]
        public void LineInverseSlopeHorizontalIsInfiite()
        {
            Vector2 v = new Vector2(1, 0);

            float res = Vector2.InverseSlope(Vector2.Zero, v);

            Assert.That(float.IsInfinity(res));
        }

        [TestCase(0, 1, 0)]
        [TestCase(1, 1, 1)]
        [TestCase(2, 1, 2)]
        [TestCase(1, 2, 0.5f)]
        public void LineInverseSlopeWorks(float x, float y, float expected)
        {
            Vector2 v = new Vector2(x, y);

            float res = Vector2.InverseSlope(Vector2.Zero, v);

            Assert.That(res, Is.EqualTo(expected));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void LineInterceptWorks(float f)
        {
            Vector2 v2 = new Vector2(1, f + 1);
            float b = v2.Intercept(1);

            Assert.That(b, Is.EqualTo(f));
        }
    }
}