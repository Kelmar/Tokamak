using System.Numerics;
using System.Runtime.InteropServices;

namespace Tokamak.Formats
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VectorFormatPC
    {
        [FormatDescriptor(FormatBaseType.Float, 3)]
        public Vector3 Point;

        [FormatDescriptor(FormatBaseType.Float, 4)]
        public Vector4 Color;
    }
}
