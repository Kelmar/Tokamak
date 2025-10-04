using System;
using System.Numerics;

namespace Tokamak.Mathematics
{
    /// <summary>
    /// Extensions to System.Numerics matrix classes.
    /// </summary>
    public static class MatrixEx
    {
        /// <summary>
        /// Create a 3x2 matrix that skews along the X axis.
        /// </summary>
        public static Matrix3x2 CreateSkewX(float a)
        {
            Matrix3x2 rval = Matrix3x2.Identity;
            rval.M21 = MathF.Tan(a);

            return rval;
        }

        /// <summary>
        /// Create a 3x2 matrix that skews along the Y axis.
        /// </summary>
        public static Matrix3x2 CreateSkewY(float a)
        {
            Matrix3x2 rval = Matrix3x2.Identity;
            rval.M12 = MathF.Tan(a);

            return rval;
        }

    }
}
