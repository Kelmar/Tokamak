using System.Numerics;
using System.Runtime.InteropServices;

namespace Tokamak.Formats
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VectorFormatPCT
    {
        [FormatDescriptor(FormatBaseType.Float, 3)]
        public Vector3 Point;

        [FormatDescriptor(FormatBaseType.Float, 3)]
        public Vector4 Color;

        [FormatDescriptor(FormatBaseType.Float, 2)]
        public Vector2 TexCoord;
    }
}
