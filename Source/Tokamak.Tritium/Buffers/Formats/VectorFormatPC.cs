using System.Numerics;
using System.Runtime.InteropServices;

namespace Tokamak.Tritium.Buffers.Formats
{
    /// <summary>
    /// Format containing vertex positional info and color details.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VectorFormatPC
    {
        /// <summary>
        /// Vertex position in 3D space.
        /// </summary>
        [FormatDescriptor(FormatBaseType.Float, 3)]
        public Vector3 Point;

        /// <summary>
        /// Color with alpha blending.
        /// </summary>
        /// <remarks>
        /// Values are floating point from 0 to 1.
        /// </remarks>
        [FormatDescriptor(FormatBaseType.Float, 4)]
        public Vector4 Color;
    }
}
