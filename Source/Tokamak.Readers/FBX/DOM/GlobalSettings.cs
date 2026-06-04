using System.Numerics;

using Tokamak.Mathematics;

namespace Tokamak.Readers.FBX.DOM
{
    internal class GlobalSettings
    {
        public int UpAxis { get; set; } = 1; // Default Y

        public int UpAxisSign { get; set; } = 1;

        public int FrontAxis { get; set; } = 2; // Default Z

        public int FrontAxisSign { get; set; } = 1;

        public int CoordAxis { get; set; } = 0; // Default X

        public int CoordAxisSign { get; set; } = 1;

        public Matrix4x4 AxisSwizzleMatrix { get; set; } = Matrix4x4.Identity;

        private Vector4 GetAxisVector(int axis)
        {
            float[] data = [0, 0, 0, 0];
            data[axis] = 1;

            return VectorEx.ToVector4(data);
        }

        public void BuildSwizzleMatrix()
        {
            AxisSwizzleMatrix = Matrix4x4.Create(
                GetAxisVector(CoordAxis) * CoordAxisSign,
                GetAxisVector(UpAxis) * UpAxisSign,
                GetAxisVector(FrontAxis) * FrontAxisSign,
                new Vector4(0, 0, 0, 1)
            );
        }

        public Vector3 SwizzleAxes(Vector3 v)
            => Vector3.Transform(v, AxisSwizzleMatrix);

        private void ValidateAxis(int id, string name)
        {
            if (id >= 0 && id < 3)
                return;

            throw new System.Exception($"Invalid global {name} axis index in FBX file.");
        }

        public void Validate()
        {
            ValidateAxis(UpAxis, "up");
            ValidateAxis(FrontAxis, "front");
            ValidateAxis(CoordAxis, "coord");

            BuildSwizzleMatrix();
        }
    }
}
