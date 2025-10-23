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
    internal class Bezier3DTests
    {
        public static Vector3[] TestVectors =
        [
            new Vector3(50, 50, 50),
            new Vector3(100, 100, 100),
            new Vector3(150, 50, 125),
            new Vector3(200, 100, 300)
        ];

        public static float[] TestDeltas = [0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f];

        [TestCase]
        public void QuadradicBezierGetsFirstVectorAtZero()
        {
            Vector3 t = Bezier.QuadSolve(TestVectors[0], TestVectors[1], TestVectors[2], 0);

            Assert.That(t, Is.EqualTo(TestVectors[0]));
        }

        [TestCase]
        public void QuadradicBezierGetsLastVectorAtOne()
        {
            Vector3 t = Bezier.QuadSolve(TestVectors[0], TestVectors[1], TestVectors[2], 1);

            Assert.That(t, Is.EqualTo(TestVectors[2]));
        }

        [Ignore("SolveN() can be highly inaccurate, use this test only if absolutely needed.")]
        [TestCaseSource(nameof(TestDeltas))]
        public void QuadradicBezierMatchesSolveN(float delta)
        {
            Vector3 solveNRes = Bezier.SolveN(TestVectors.Take(3), delta);
            Vector3 t = Bezier.QuadSolve(TestVectors[0], TestVectors[1], TestVectors[2], delta);

            Assert.That(t.X, Is.InRange(solveNRes.X - 0.0001f, solveNRes.X + 0.0001f));
            Assert.That(t.Y, Is.InRange(solveNRes.Y - 0.0001f, solveNRes.Y + 0.0001f));
            Assert.That(t.Z, Is.InRange(solveNRes.Z - 0.0001f, solveNRes.Z + 0.0001f));
        }

        [Ignore("SolveN() can be highly inaccurate, use this test only if absolutely needed.")]
        [TestCaseSource(nameof(TestDeltas))]
        public void CubicBezierMatchesSolveN(float delta)
        {
            Vector3 solveNRes = Bezier.SolveN(TestVectors, delta);
            Vector3 t = Bezier.CubicSolve(TestVectors[0], TestVectors[1], TestVectors[2], TestVectors[3], delta);

            Assert.That(t.X, Is.InRange(solveNRes.X - 0.0001f, solveNRes.X + 0.0001f));
            Assert.That(t.Y, Is.InRange(solveNRes.Y - 0.0001f, solveNRes.Y + 0.0001f));
            Assert.That(t.Z, Is.InRange(solveNRes.Z - 0.0001f, solveNRes.Z + 0.0001f));
        }

        [TestCase]
        public void CubicBezierGetsFirstVectorAtZero()
        {
            Vector3 t = Bezier.CubicSolve(TestVectors[0], TestVectors[1], TestVectors[2], TestVectors[3], 0);

            Assert.That(t, Is.EqualTo(TestVectors[0]));
        }

        [TestCase]
        public void CubicBezierGetsLastVectorAtOne()
        {
            Vector3 t = Bezier.CubicSolve(TestVectors[0], TestVectors[1], TestVectors[2], TestVectors[3], 1);

            Assert.That(t, Is.EqualTo(TestVectors[3]));
        }
    }
}
