using System;

using NUnit.Framework;

using Tokamak.Mathematics;

namespace MathTests
{
    public class MathXTests
    {
        public static (int Value, int Expected)[] FactorTests = 
        [
            (0,  1        ),
            (1,  1        ),
            (2,  2        ),
            (3,  6        ),
            (4,  24       ),
            (5,  120      ),
            (6,  720      ),
            (7,  5_040    ),
            (8,  40_320   ),
            (9,  362_880  ),
            (10, 3_628_800)
        ];

        public static float[] FloatLerps = [0, 0.5f, 1];
        public static double[] DoubleLerps = [0, 0.5, 1];

        [TestCaseSource(nameof(FloatLerps))]
        public void FloatLerp(float value)
        {
            float result = MathX.LerpF(0, 1, value);

            Assert.That(result, Is.InRange(value - MathX.FUZ, value + MathX.FUZ));
        }

        [TestCaseSource(nameof(DoubleLerps))]
        public void DoubleLerp(double value)
        {
            double result = MathX.Lerp(0, 1, value);

            Assert.That(result, Is.InRange(value - MathX.FUZ, value + MathX.FUZ));
        }

        [TestCaseSource(nameof(FactorTests))]
        public void Factors((int Value, int Expected) testCase)
        {
            int res = MathX.Factorial(testCase.Value);
            Assert.That(res, Is.EqualTo(testCase.Expected));
        }
    }
}
