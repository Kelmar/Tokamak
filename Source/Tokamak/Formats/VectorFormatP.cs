using System.Numerics;
using System.Runtime.InteropServices;

namespace Tokamak.Formats
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VectorFormatP
    {
        [FormatDescriptor(FormatBaseType.Float, 3)]
        public Vector3 Point;
    }
}
