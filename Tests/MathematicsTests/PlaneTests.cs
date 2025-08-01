using System.Numerics;

using NUnit.Framework;

using Tokamak.Mathematics;

namespace MathTests
{
    public class PlaneTests
    {
#region Side Test Cases
        public class SideCase
        {
            public Vector3 Vector { get; set; }

            public Plane Plane { get; set; }

            public Boundary Expected { get; set; }

            public override string ToString() => $"v:{Vector}, p:{Plane}, {Expected}";
        }

        public static SideCase[] SideCases =
        {
            new SideCase { Vector = new Vector3(), Plane = new Plane(), Expected = Boundary.On },
            new SideCase
            {
                Vector = new Vector3(),
                Plane = new Plane(Vector3.UnitY, 1),
                Expected = Boundary.Back
            },
            new SideCase
            {
                Vector = new Vector3(0, 2, 0),
                Plane = new Plane(Vector3.UnitY, 1),
                Expected = Boundary.Front
            },
            new SideCase
            {
                Vector = new Vector3(100, 1.1f, 100),
                Plane = new Plane(Vector3.UnitY, 1),
                Expected = Boundary.Front
            }
        };
#endregion Side Test Cases

#region Distance Test Cases
        public class DistanceCase
        {
            public Vector3 Vector { get; set; }

            public Plane Plane { get; set; }

            public float Expected { get; set; }

            public override string ToString() => $"v:{Vector}, p:{Plane}, {Expected}";
        }

        public static DistanceCase[] DistanceCases =
        {
            new DistanceCase { Vector = new Vector3(), Plane = new Plane(), Expected = 0 },
            new DistanceCase
            {
                Vector = new Vector3(),
                Plane = new Plane(Vector3.UnitY, 1),
                Expected = 1
            },
            new DistanceCase
            {
                Vector = new Vector3(0, 2, 0),
                Plane = new Plane(Vector3.UnitY, 1),
                Expected = 1
            },
            new DistanceCase
            {
                Vector = new Vector3(100, 1.1f, 100),
                Plane = new Plane(Vector3.UnitY, 1),
                Expected = 1.1f
            }
        };
#endregion

        [TestCaseSource(nameof(SideCases))]
        public void VectorSideTests(SideCase conditions)
        {
            Boundary result = conditions.Plane.WhichSide(conditions.Vector);

            Assert.That(result, Is.EqualTo(conditions.Expected));
        }

        [TestCaseSource(nameof(DistanceCases))]
        public void VectorDistranceTests(DistanceCase conditions)
        {
            float result = conditions.Plane.DistanceTo(conditions.Vector);

            float fuz1 = result - MathX.FUZ;
            float fuz2 = result + MathX.FUZ;

            Assert.That(result, Is.GreaterThanOrEqualTo(fuz1).And.LessThanOrEqualTo(fuz2));
        }
    }
}
