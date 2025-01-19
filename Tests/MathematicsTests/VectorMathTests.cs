using System.Numerics;

using NUnit.Framework;

using Tokamak.Mathematics;

namespace MathTests
{
    public class VectorMathTests
    {
        //[SetUp]
        //public void Setup()
        //{
        //}

        [Test]
        public void CrossProductWorks()
        {
            Vector2 v1 = new Vector2(3, -5);
            Vector2 v2 = new Vector2(7, 5);

            float res = MathX.Cross(v1, v2);

            Assert.That(res, Is.EqualTo(50));
        }
    }
}