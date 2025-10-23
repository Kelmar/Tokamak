using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Tokamak.Mathematics;

namespace MathTests
{
    internal class Bezier2DTests
    {
        public static Vector2[] TestVectors =
        [
            new Vector2(50, 50),
            new Vector2(100, 100),
            new Vector2(150, 50),
            new Vector2(200, 100)
        ];

        public static float[] TestDeltas = [0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f];

        [TestCase]
        public void QuadradicBezierGetsFirstVectorAtZero()
        {
            Vector2 t = Bezier.QuadSolve(TestVectors[0], TestVectors[1], TestVectors[2], 0);

            Assert.That(t, Is.EqualTo(TestVectors[0]));
        }

        [TestCase]
        public void QuadradicBezierGetsLastVectorAtOne()
        {
            Vector2 t = Bezier.QuadSolve(TestVectors[0], TestVectors[1], TestVectors[2], 1);

            Assert.That(t, Is.EqualTo(TestVectors[2]));
        }

        [Ignore("SolveN() can be highly inaccurate, use this test only if absolutely needed.")]
        [TestCaseSource(nameof(TestDeltas))]
        public void QuadradicBezierMatchesSolveN(float delta)
        {
            Vector2 solveNRes = Bezier.SolveN(TestVectors.Take(3), delta);
            Vector2 t = Bezier.QuadSolve(TestVectors[0], TestVectors[1], TestVectors[2], delta);

            Assert.That(t.X, Is.InRange(solveNRes.X - 0.0001f, solveNRes.X + 0.0001f));
            Assert.That(t.Y, Is.InRange(solveNRes.Y - 0.0001f, solveNRes.Y + 0.0001f));
        }

        [Ignore("SolveN() can be highly inaccurate, use this test only if absolutely needed.")]
        [TestCaseSource(nameof(TestDeltas))]
        public void CubicBezierMatchesSolveN(float delta)
        {
            Vector2 solveNRes = Bezier.SolveN(TestVectors, delta);
            Vector2 t = Bezier.CubicSolve(TestVectors[0], TestVectors[1], TestVectors[2], TestVectors[3], delta);

            Assert.That(t.X, Is.InRange(solveNRes.X - 0.0001f, solveNRes.X + 0.0001f));
            Assert.That(t.Y, Is.InRange(solveNRes.Y - 0.0001f, solveNRes.Y + 0.0001f));
        }

        [TestCase]
        public void CubicBezierGetsFirstVectorAtZero()
        {
            Vector2 t = Bezier.CubicSolve(TestVectors[0], TestVectors[1], TestVectors[2], TestVectors[3], 0);

            Assert.That(t, Is.EqualTo(TestVectors[0]));
        }

        [TestCase]
        public void CubicBezierGetsLastVectorAtOne()
        {
            Vector2 t = Bezier.CubicSolve(TestVectors[0], TestVectors[1], TestVectors[2], TestVectors[3], 1);

            Assert.That(t, Is.EqualTo(TestVectors[3]));
        }
    }
}
