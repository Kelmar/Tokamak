using System.Numerics;
using System.Runtime.InteropServices;

namespace Tokamak.Tritium.Buffers.Formats
{

    [StructLayout(LayoutKind.Sequential)]
    public struct VectorFormatPNCT
    {
        [FormatDescriptor(FormatBaseType.Float, 3)]
        public Vector3 Point;

        [FormatDescriptor(FormatBaseType.Float, 3)]
        public Vector3 Normal;

        [FormatDescriptor(FormatBaseType.Float, 4)]
        public Vector4 Color;

        [FormatDescriptor(FormatBaseType.Float, 2)]
        public Vector2 TexCoord;
    }
}
