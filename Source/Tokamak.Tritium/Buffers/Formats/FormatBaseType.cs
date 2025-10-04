namespace Tokamak.Tritium.Buffers.Formats
{
    /// <summary>
    /// The primitive type that is used to compose a more complex format type.
    /// </summary>
    /// <remarks>
    /// E.g.: double for a 3D vector
    /// </remarks>
    public enum FormatBaseType
    {
        /// <summary>
        /// Signed 8-bit integer.
        /// </summary>
        Byte,

        /// <summary>
        /// Unsigned 8-bit integer.
        /// </summary>
        UnsignedByte,

        /// <summary>
        /// Signed 16-bit integer.
        /// </summary>
        Short,

        /// <summary>
        /// Unsigned 16-bit integer.
        /// </summary>
        UnsignedShort,

        /// <summary>
        /// Signed 32-bit integer
        /// </summary>
        Int,

        /// <summary>
        /// Unsiged 32-bit integer
        /// </summary>
        UnsignedInt,

        /// <summary>
        /// Single presision 32-bit floating point.
        /// </summary>
        Float,

        /// <summary>
        /// Double presision 64-bit floating point.
        /// </summary>
        Double
    }
}
