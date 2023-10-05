using System.Numerics;

using NUnit.Framework;

using Graphite;

namespace GraphiteTests
{
    public class MathTests
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

            Assert.AreEqual(50, res);
        }
    }
}