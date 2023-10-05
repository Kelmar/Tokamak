using Mathmatic = OpenTK.Mathematics;
using Numeric = System.Numerics;

namespace Graphite.OGL
{
    public static class NumericConvert
    {
        public static Mathmatic.Vector4 ToOpenTK(this in Color color)
        {
            return new Mathmatic.Vector4(
                (float)color.Red / byte.MaxValue,
                (float)color.Green / byte.MaxValue,
                (float)color.Blue / byte.MaxValue,
                (float)color.Alpha / byte.MaxValue);
        }

        public static Mathmatic.Matrix3x2 ToOpenTK(this in Numeric.Matrix3x2 iMatrix)
        {
            return new Mathmatic.Matrix3x2
            {
                M11 = iMatrix.M11,
                M12 = iMatrix.M12,
                M21 = iMatrix.M21,
                M22 = iMatrix.M22,
                M31 = iMatrix.M31,
                M32 = iMatrix.M32
            };
        }

        public static Mathmatic.Matrix4 ToOpenTK(this in Numeric.Matrix4x4 iMatrix)
        {
            return new Mathmatic.Matrix4
            {
                M11 = iMatrix.M11,
                M12 = iMatrix.M12,
                M13 = iMatrix.M13,
                M14 = iMatrix.M14,
                M21 = iMatrix.M21,
                M22 = iMatrix.M22,
                M23 = iMatrix.M23,
                M24 = iMatrix.M24,
                M31 = iMatrix.M31,
                M32 = iMatrix.M32,
                M33 = iMatrix.M33,
                M34 = iMatrix.M34,
                M41 = iMatrix.M41,
                M42 = iMatrix.M42,
                M43 = iMatrix.M43,
                M44 = iMatrix.M44
            };
        }
    }
}
