using System.Numerics;

namespace Tokamak.Readers.FBX.ObjectWrappers
{
    internal class GlobalSettings
    {
        public int UpAxis { get; set; } = 1; // Default Y

        public int UpAxisSign { get; set; } = 1;

        public int FrontAxis { get; set; } = 2; // Default Z

        public int FrontAxisSign { get; set; } = 1;

        public int CoordAxis { get; set; } = 0; // Default X

        public int CoordAxisSign { get; set; } = 1;

        public Vector3 MapToVector(float[] values)
        {
            // TODO: This should be done using a transform matrix.

            return new Vector3(
                values[CoordAxis] * CoordAxisSign,
                values[UpAxis] * UpAxisSign,
                values[FrontAxis] * FrontAxisSign
            );
        }
    }
}
