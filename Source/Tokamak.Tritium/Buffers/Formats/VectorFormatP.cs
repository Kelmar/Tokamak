using System.Numerics;
using System.Runtime.InteropServices;

namespace Tokamak.Tritium.Buffers.Formats
{
    /// <summary>
    /// Format containing only vertex positional info.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VectorFormatP
    {
        /// <summary>
        /// Vertex position in 3D space.
        /// </summary>
        [FormatDescriptor(FormatBaseType.Float, 3)]
        public Vector3 Point;
    }
}
