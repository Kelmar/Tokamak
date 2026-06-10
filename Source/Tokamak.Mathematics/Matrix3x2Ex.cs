using System;
using System.Numerics;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Extensions to <seealso cref="System.Numerics"/> matrix classes.
    /// </summary>
    public static class Matrix3x2Ex
    {
        extension(in Matrix3x2 mat)
        {
            /// <summary>
            /// Create a <seealso cref="Matrix3x2"/> that skews along the X axis.
            /// </summary>
            /// <param name="a">How far along the X axis to skew.</param>
            public static Matrix3x2 CreateSkewX(float a)
            {
                Matrix3x2 rval = Matrix3x2.Identity;
                rval.M21 = MathF.Tan(a);

                return rval;
            }

            /// <summary>
            /// Create a <seealso cref="Matrix3x2"/> that skews along the Y axis.
            /// </summary>
            /// <param name="a">How far along the Y axis to skew.</param>
            public static Matrix3x2 CreateSkewY(float a)
            {
                Matrix3x2 rval = Matrix3x2.Identity;
                rval.M12 = MathF.Tan(a);

                return rval;
            }

            /// <summary>
            /// Create a 3x2 matrix from row major data.
            /// </summary>
            /// <param name="a">Array of row major data.</param>
            public static Matrix3x2 CreateFromRowArray(float[] a)
            {
                return new Matrix3x2(
                    a.Length > 0 ? a[0] : 0,
                    a.Length > 3 ? a[3] : 0,
                    a.Length > 1 ? a[1] : 0,
                    a.Length > 4 ? a[4] : 0,
                    a.Length > 2 ? a[2] : 0,
                    a.Length > 5 ? a[5] : 0);
            }

            /// <summary>
            /// Create a 3x2 matrix from column major data.
            /// </summary>
            /// <param name="a">Array of column major data.</param>
            public static Matrix3x2 CreateFromColumnArray(float[] a)
            {
                return new Matrix3x2(
                    a.Length > 0 ? a[0] : 0,
                    a.Length > 1 ? a[1] : 0,
                    a.Length > 2 ? a[2] : 0,
                    a.Length > 3 ? a[3] : 0,
                    a.Length > 4 ? a[4] : 0,
                    a.Length > 5 ? a[5] : 0);
            }
        }
    }
}
