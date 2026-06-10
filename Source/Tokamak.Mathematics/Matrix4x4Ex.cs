using System.Numerics;

namespace Tokamak.Mathematics
{
    public static class Matrix4x4Ex
    {
        extension(in Matrix4x4 mat)
        {
            /// <summary>
            /// Create a 4x4 matrix from row major data.
            /// </summary>
            /// <param name="a">Array of row major data.</param>
            public static Matrix4x4 CreateFromRowArray(float[] a)
            {
                return new Matrix4x4(
                    a.Length > 0  ? a[0]  : 0,
                    a.Length > 4  ? a[4]  : 0,
                    a.Length > 8  ? a[8]  : 0,
                    a.Length > 12 ? a[12] : 0,

                    a.Length > 1  ? a[1]  : 0,
                    a.Length > 5  ? a[5]  : 0,
                    a.Length > 9  ? a[9]  : 0,
                    a.Length > 13 ? a[13] : 0,

                    a.Length > 2  ? a[2]  : 0,
                    a.Length > 6  ? a[6]  : 0,
                    a.Length > 10 ? a[10] : 0,
                    a.Length > 14 ? a[14] : 0,

                    a.Length > 3  ? a[3]  : 0,
                    a.Length > 7  ? a[7]  : 0,
                    a.Length > 11 ? a[11] : 0,
                    a.Length > 15 ? a[15] : 0);
            }

            /// <summary>
            /// Create a 4x4 matrix from column major data.
            /// </summary>
            /// <param name="a">Array of column major data.</param>
            public static Matrix4x4 CreateFromColumnArray(float[] a)
            {
                return new Matrix4x4(
                    a.Length > 0  ? a[0]  : 0,
                    a.Length > 1  ? a[1]  : 0,
                    a.Length > 2  ? a[2]  : 0,
                    a.Length > 3  ? a[3]  : 0,
                    a.Length > 4  ? a[4]  : 0,
                    a.Length > 5  ? a[5]  : 0,
                    a.Length > 6  ? a[6]  : 0,
                    a.Length > 7  ? a[7]  : 0,
                    a.Length > 8  ? a[8]  : 0,
                    a.Length > 9  ? a[9]  : 0,
                    a.Length > 10 ? a[10] : 0,
                    a.Length > 11 ? a[11] : 0,
                    a.Length > 12 ? a[12] : 0,
                    a.Length > 13 ? a[13] : 0,
                    a.Length > 14 ? a[14] : 0,
                    a.Length > 15 ? a[15] : 0);
            }
        }
    }
}
