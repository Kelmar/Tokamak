using System;

using NUnit.Framework;

using Tokamak.Mathematics;

namespace MathTests
{
    public class MathXTests
    {
        public static (int Value, int Expected)[] FactorialCases = 
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

        public static (int Value, int Expected )[] NextPow2Cases =
        [
            (0, 1),
            (1, 2),
            (2, 4),
            (3, 4),
            (4, 8),
            (5, 8),
            (6, 8),
            (7, 8),
            (8, 16),
            (16, 32),
            (32, 64),
            (320, 512),
            (640, 1024),
            (1024, 2048),
            (2048, 4096)
        ];

        [TestCaseSource(nameof(FactorialCases))]
        public void TestFactorial((int Value, int Expected) testCase)
        {
            int res = MathX.Factorial(testCase.Value);

            Assert.That(res, Is.EqualTo(testCase.Expected));
        }

        [TestCaseSource(nameof(NextPow2Cases))]
        public void TestNextPow2((int Value, int Expected) testCase)
        {
            int res = MathX.NextPow2(testCase.Value);

            Assert.That(res, Is.EqualTo(testCase.Expected));
        }
    }
}
