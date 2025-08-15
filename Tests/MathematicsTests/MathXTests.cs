using NUnit.Framework;

using Tokamak.Mathematics;

namespace MathTests
{
    public class MathXTests
    {

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
    }
}
