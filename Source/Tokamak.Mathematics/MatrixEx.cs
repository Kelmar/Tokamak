using System;
using System.Numerics;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Extensions to <seealso cref="System.Numerics"/> matrix classes.
    /// </summary>
    public static class MatrixEx
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
        }
    }
}
